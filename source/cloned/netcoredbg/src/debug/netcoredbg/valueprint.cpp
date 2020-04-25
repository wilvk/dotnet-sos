// Copyright (c) 2017 Samsung Electronics Co., LTD
// Distributed under the MIT License.
// See the LICENSE file in the project root for more information.

#include "valueprint.h"

#include <sstream>
#include <vector>
#include <iomanip>

#include <arrayholder.h>

#include "typeprinter.h"
#include "torelease.h"
#include "cputil.h"
#include "symbolreader.h"

using std::string;

// From strike.cpp
HRESULT DereferenceAndUnboxValue(ICorDebugValue * pValue, ICorDebugValue** ppOutputValue, BOOL * pIsNull)
{
    HRESULT Status = S_OK;
    *ppOutputValue = nullptr;
    if (pIsNull != nullptr) *pIsNull = FALSE;

    ToRelease<ICorDebugReferenceValue> pReferenceValue;
    Status = pValue->QueryInterface(IID_ICorDebugReferenceValue, (LPVOID*) &pReferenceValue);
    if (SUCCEEDED(Status))
    {
        BOOL isNull = FALSE;
        IfFailRet(pReferenceValue->IsNull(&isNull));
        if(!isNull)
        {
            ToRelease<ICorDebugValue> pDereferencedValue;
            IfFailRet(pReferenceValue->Dereference(&pDereferencedValue));
            return DereferenceAndUnboxValue(pDereferencedValue, ppOutputValue, pIsNull);
        }
        else
        {
            if(pIsNull != nullptr) *pIsNull = TRUE;
            *ppOutputValue = pValue;
            (*ppOutputValue)->AddRef();
            return S_OK;
        }
    }

    ToRelease<ICorDebugBoxValue> pBoxedValue;
    Status = pValue->QueryInterface(IID_ICorDebugBoxValue, (LPVOID*) &pBoxedValue);
    if (SUCCEEDED(Status))
    {
        ToRelease<ICorDebugObjectValue> pUnboxedValue;
        IfFailRet(pBoxedValue->GetObject(&pUnboxedValue));
        return DereferenceAndUnboxValue(pUnboxedValue, ppOutputValue, pIsNull);
    }
    *ppOutputValue = pValue;
    (*ppOutputValue)->AddRef();
    return S_OK;
}

static bool IsEnum(ICorDebugValue *pInputValue)
{
    ToRelease<ICorDebugValue> pValue;
    if (FAILED(DereferenceAndUnboxValue(pInputValue, &pValue, nullptr))) return false;

    string baseTypeName;
    ToRelease<ICorDebugValue2> pValue2;
    ToRelease<ICorDebugType> pType;
    ToRelease<ICorDebugType> pBaseType;

    if (FAILED(pValue->QueryInterface(IID_ICorDebugValue2, (LPVOID *) &pValue2))) return false;
    if (FAILED(pValue2->GetExactType(&pType))) return false;
    if (FAILED(pType->GetBase(&pBaseType)) || pBaseType == nullptr) return false;
    if (FAILED(TypePrinter::GetTypeOfValue(pBaseType, baseTypeName))) return false;

    return baseTypeName == "System.Enum";
}

static HRESULT PrintEnumValue(ICorDebugValue* pInputValue, BYTE* enumValue, string &output)
{
    HRESULT Status = S_OK;

    ToRelease<ICorDebugValue> pValue;
    IfFailRet(DereferenceAndUnboxValue(pInputValue, &pValue, nullptr));

    mdTypeDef currentTypeDef;
    ToRelease<ICorDebugClass> pClass;
    ToRelease<ICorDebugValue2> pValue2;
    ToRelease<ICorDebugType> pType;
    ToRelease<ICorDebugModule> pModule;
    IfFailRet(pValue->QueryInterface(IID_ICorDebugValue2, (LPVOID *) &pValue2));
    IfFailRet(pValue2->GetExactType(&pType));
    IfFailRet(pType->GetClass(&pClass));
    IfFailRet(pClass->GetModule(&pModule));
    IfFailRet(pClass->GetToken(&currentTypeDef));

    ToRelease<IUnknown> pMDUnknown;
    ToRelease<IMetaDataImport> pMD;
    IfFailRet(pModule->GetMetaDataInterface(IID_IMetaDataImport, &pMDUnknown));
    IfFailRet(pMDUnknown->QueryInterface(IID_IMetaDataImport, (LPVOID*) &pMD));


    //First, we need to figure out the underlying enum type so that we can correctly type cast the raw values of each enum constant
    //We get that from the non-static field of the enum variable (I think the field is called __value or something similar)
    ULONG numFields = 0;
    HCORENUM fEnum = NULL;
    mdFieldDef fieldDef;
    CorElementType enumUnderlyingType = ELEMENT_TYPE_END;
    while(SUCCEEDED(pMD->EnumFields(&fEnum, currentTypeDef, &fieldDef, 1, &numFields)) && numFields != 0)
    {
        DWORD             fieldAttr = 0;
        PCCOR_SIGNATURE   pSignatureBlob = NULL;
        ULONG             sigBlobLength = 0;
        if(SUCCEEDED(pMD->GetFieldProps(fieldDef, NULL, NULL, 0, NULL, &fieldAttr, &pSignatureBlob, &sigBlobLength, NULL, NULL, NULL)))
        {
            if((fieldAttr & fdStatic) == 0)
            {
                CorSigUncompressCallingConv(pSignatureBlob);
                enumUnderlyingType = CorSigUncompressElementType(pSignatureBlob);
                break;
            }
        }
    }
    pMD->CloseEnum(fEnum);

    std::ostringstream ss;
    const char *sep = "";

    //Now that we know the underlying enum type, let's decode the enum variable into OR-ed, human readable enum constants
    fEnum = NULL;
    bool isFirst = true;
    ULONG64 remainingValue = *((ULONG64*)enumValue);
    while(SUCCEEDED(pMD->EnumFields(&fEnum, currentTypeDef, &fieldDef, 1, &numFields)) && numFields != 0)
    {
        ULONG             nameLen = 0;
        DWORD             fieldAttr = 0;
        WCHAR             mdName[mdNameLen];
        UVCP_CONSTANT     pRawValue = NULL;
        ULONG             rawValueLength = 0;
        if(SUCCEEDED(pMD->GetFieldProps(fieldDef, NULL, mdName, mdNameLen, &nameLen, &fieldAttr, NULL, NULL, NULL, &pRawValue, &rawValueLength)))
        {
            DWORD enumValueRequiredAttributes = fdPublic | fdStatic | fdLiteral | fdHasDefault;
            if((fieldAttr & enumValueRequiredAttributes) != enumValueRequiredAttributes)
                continue;

            ULONG64 currentConstValue = 0;
            switch (enumUnderlyingType)
            {
                case ELEMENT_TYPE_CHAR:
                case ELEMENT_TYPE_I1:
                    currentConstValue = (ULONG64)(*((CHAR*)pRawValue));
                    break;
                case ELEMENT_TYPE_U1:
                    currentConstValue = (ULONG64)(*((BYTE*)pRawValue));
                    break;
                case ELEMENT_TYPE_I2:
                    currentConstValue = (ULONG64)(*((SHORT*)pRawValue));
                    break;
                case ELEMENT_TYPE_U2:
                    currentConstValue = (ULONG64)(*((USHORT*)pRawValue));
                    break;
                case ELEMENT_TYPE_I4:
                    currentConstValue = (ULONG64)(*((INT32*)pRawValue));
                    break;
                case ELEMENT_TYPE_U4:
                    currentConstValue = (ULONG64)(*((UINT32*)pRawValue));
                    break;
                case ELEMENT_TYPE_I8:
                    currentConstValue = (ULONG64)(*((LONG*)pRawValue));
                    break;
                case ELEMENT_TYPE_U8:
                    currentConstValue = (ULONG64)(*((ULONG*)pRawValue));
                    break;
                case ELEMENT_TYPE_I:
                    currentConstValue = (ULONG64)(*((int*)pRawValue));
                    break;
                case ELEMENT_TYPE_U:
                case ELEMENT_TYPE_R4:
                case ELEMENT_TYPE_R8:
                // Technically U and the floating-point ones are options in the CLI, but not in the CLS or C#, so these are NYI
                default:
                    currentConstValue = 0;
            }

            if((currentConstValue == remainingValue) || ((currentConstValue != 0) && ((currentConstValue & remainingValue) == currentConstValue)))
            {
                remainingValue &= ~currentConstValue;

                ss << sep;
                sep = " | ";
                ss << to_utf8(mdName);
            }
        }
    }
    pMD->CloseEnum(fEnum);

    output = ss.str();

    return S_OK;
}

static HRESULT GetUIntValue(ICorDebugValue *pInputValue, unsigned int &value)
{
    HRESULT Status;

    BOOL isNull = TRUE;
    ToRelease<ICorDebugValue> pValue;
    IfFailRet(DereferenceAndUnboxValue(pInputValue, &pValue, &isNull));

    if(isNull)
        return E_FAIL;

    ULONG32 cbSize;
    IfFailRet(pValue->GetSize(&cbSize));
    if (cbSize != sizeof(int))
        return E_FAIL;

    BYTE rgbValue[sizeof(int)] = {0};

    ToRelease<ICorDebugGenericValue> pGenericValue;
    IfFailRet(pValue->QueryInterface(IID_ICorDebugGenericValue, (LPVOID*) &pGenericValue));
    IfFailRet(pGenericValue->GetValue((LPVOID) &(rgbValue[0])));

    CorElementType corElemType;
    IfFailRet(pValue->GetType(&corElemType));

    switch (corElemType)
    {
    default:
        return E_FAIL;
    case ELEMENT_TYPE_I4:
    case ELEMENT_TYPE_U4:
        value = *(unsigned int*) &(rgbValue[0]);
        return S_OK;
    }
    return E_FAIL;
}

static HRESULT GetDecimalFields(ICorDebugValue *pValue,
                                unsigned int &hi,
                                unsigned int &mid,
                                unsigned int &lo,
                                unsigned int &flags)
{
    HRESULT Status = S_OK;

    mdTypeDef currentTypeDef;
    ToRelease<ICorDebugClass> pClass;
    ToRelease<ICorDebugValue2> pValue2;
    ToRelease<ICorDebugType> pType;
    ToRelease<ICorDebugModule> pModule;
    IfFailRet(pValue->QueryInterface(IID_ICorDebugValue2, (LPVOID *) &pValue2));
    IfFailRet(pValue2->GetExactType(&pType));
    IfFailRet(pType->GetClass(&pClass));
    IfFailRet(pClass->GetModule(&pModule));
    IfFailRet(pClass->GetToken(&currentTypeDef));
    ToRelease<IUnknown> pMDUnknown;
    ToRelease<IMetaDataImport> pMD;
    IfFailRet(pModule->GetMetaDataInterface(IID_IMetaDataImport, &pMDUnknown));
    IfFailRet(pMDUnknown->QueryInterface(IID_IMetaDataImport, (LPVOID*) &pMD));

    bool has_hi = false;
    bool has_mid = false;
    bool has_lo = false;
    bool has_flags = false;

    ULONG numFields = 0;
    HCORENUM fEnum = NULL;
    mdFieldDef fieldDef;
    while(SUCCEEDED(pMD->EnumFields(&fEnum, currentTypeDef, &fieldDef, 1, &numFields)) && numFields != 0)
    {
        ULONG nameLen = 0;
        DWORD fieldAttr = 0;
        WCHAR mdName[mdNameLen] = {0};
        if(SUCCEEDED(pMD->GetFieldProps(fieldDef, NULL, mdName, mdNameLen, &nameLen, &fieldAttr, NULL, NULL, NULL, NULL, NULL)))
        {
            if(fieldAttr & fdLiteral)
                continue;
            if (fieldAttr & fdStatic)
                continue;

            ToRelease<ICorDebugValue> pFieldVal;
            ToRelease<ICorDebugObjectValue> pObjValue;
            IfFailRet(pValue->QueryInterface(IID_ICorDebugObjectValue, (LPVOID*) &pObjValue));
            IfFailRet(pObjValue->GetFieldValue(pClass, fieldDef, &pFieldVal));

            string name = to_utf8(mdName /*, nameLen*/);

            if (name == "hi")
            {
                IfFailRet(GetUIntValue(pFieldVal, hi));
                has_hi = true;
            }
            else if (name == "mid")
            {
                IfFailRet(GetUIntValue(pFieldVal, mid));
                has_mid = true;
            } else if (name == "lo")
            {
                IfFailRet(GetUIntValue(pFieldVal, lo));
                has_lo = true;
            }
            else if (name == "flags")
            {
                IfFailRet(GetUIntValue(pFieldVal, flags));
                has_flags = true;
            }
        }
    }
    pMD->CloseEnum(fEnum);

    return (has_hi && has_mid && has_lo && has_flags ? S_OK : E_FAIL);
}

static inline uint64_t Make_64(uint32_t h, uint32_t l) { uint64_t v = h; v <<= 32; v |= l; return v; }
static inline uint32_t Lo_32(uint64_t v) { return static_cast<uint32_t>(v); }

bool uint96_is_zero(const uint32_t *v) { return v[0] == 0 && v[1] == 0 && v[2] == 0; }

static void udivrem96(uint32_t *divident, uint32_t divisor, uint32_t &remainder)
{
    remainder = 0;
    for (int i = 2; i >= 0; i--)
    {
        uint64_t partial_dividend = Make_64(remainder, divident[i]);
        if (partial_dividend == 0) {
            divident[i] = 0;
            remainder = 0;
        } else if (partial_dividend < divisor) {
            divident[i] = 0;
            remainder = Lo_32(partial_dividend);
        } else if (partial_dividend == divisor) {
            divident[i] = 1;
            remainder = 0;
        } else {
            divident[i] = Lo_32(partial_dividend / divisor);
            remainder = Lo_32(partial_dividend - (divident[i] * divisor));
        }
    }
}

static string uint96_to_string(uint32_t *v)
{
    static const char *digits = "0123456789";
    string result;
    do {
        uint32_t rem;
        udivrem96(v, 10, rem);
        result.insert(0, 1, digits[rem]);
    } while (!uint96_is_zero(v));
    return result;
}

static void PrintDecimal(
    unsigned int hi,
    unsigned int mid,
    unsigned int lo,
    unsigned int flags,
    string &output)
{
    uint32_t v[3] = { lo, mid, hi };

    output = uint96_to_string(v);

    static const unsigned int ScaleMask = 0x00FF0000ul;
    static const unsigned int ScaleShift = 16;
    static const unsigned int SignMask = 1ul << 31;

    unsigned int scale = (flags & ScaleMask) >> ScaleShift;
    bool is_negative = flags & SignMask;

    size_t len = output.length();

    if (len > scale)
    {
        if (scale != 0)
            output.insert(len - scale, 1, '.');
    }
    else
    {
        output.insert(0, "0.");
        output.insert(2, scale - len, '0');
    }

    if (is_negative)
        output.insert(0, 1, '-');
}

static HRESULT PrintDecimalValue(ICorDebugValue *pValue,
                                 string &output)
{
    HRESULT Status = S_OK;

    unsigned int hi;
    unsigned int mid;
    unsigned int lo;
    unsigned int flags;

    IfFailRet(GetDecimalFields(pValue, hi, mid, lo, flags));

    PrintDecimal(hi, mid, lo, flags, output);

    return S_OK;
}

PACK_BEGIN struct Decimal {
    uint32_t flags;
    uint32_t hi;
    uint32_t lo;
    uint32_t mid;
} PACK_END;

static void PrintDecimalValue(const string &rawValue,
                              string &output)
{
    const Decimal *d = reinterpret_cast<const Decimal*>(&rawValue[0]);
    PrintDecimal(d->hi, d->mid, d->lo, d->flags, output);
}

static HRESULT PrintArrayValue(ICorDebugValue *pValue,
                               string &output)
{
    HRESULT Status = S_OK;

    ToRelease<ICorDebugArrayValue> pArrayValue;
    IfFailRet(pValue->QueryInterface(IID_ICorDebugArrayValue, (LPVOID*) &pArrayValue));

    ULONG32 nRank;
    IfFailRet(pArrayValue->GetRank(&nRank));
    if (nRank < 1)
    {
        return E_UNEXPECTED;
    }

    ULONG32 cElements;
    IfFailRet(pArrayValue->GetCount(&cElements));

    std::ostringstream ss;
    ss << "{";

    string elementType;
    string arrayType;

    ToRelease<ICorDebugType> pFirstParameter;
    ToRelease<ICorDebugValue2> pValue2;
    ToRelease<ICorDebugType> pType;
    if (SUCCEEDED(pArrayValue->QueryInterface(IID_ICorDebugValue2, (LPVOID *) &pValue2)) && SUCCEEDED(pValue2->GetExactType(&pType)))
    {
        if (SUCCEEDED(pType->GetFirstTypeParameter(&pFirstParameter)))
        {
            TypePrinter::GetTypeOfValue(pFirstParameter, elementType, arrayType);
        }
    }

    std::vector<ULONG32> dims(nRank, 0);
    pArrayValue->GetDimensions(nRank, &dims[0]);

    std::vector<ULONG32> base(nRank, 0);
    BOOL hasBaseIndicies = FALSE;
    if (SUCCEEDED(pArrayValue->HasBaseIndicies(&hasBaseIndicies)) && hasBaseIndicies)
        IfFailRet(pArrayValue->GetBaseIndicies(nRank, &base[0]));

    ss << elementType << "[";
    const char *sep = "";
    for (size_t i = 0; i < dims.size(); ++i)
    {
        ss << sep;
        sep = ", ";

        if (base[i] > 0)
            ss << base[i] << ".." << (base[i] + dims[i] - 1);
        else
            ss << dims[i];
    }
    ss << "]" << arrayType;

    ss << "}";
    output = ss.str();
    return S_OK;
}

static HRESULT PrintStringValue(ICorDebugValue * pValue, string &output)
{
    HRESULT Status;

    ToRelease<ICorDebugStringValue> pStringValue;
    IfFailRet(pValue->QueryInterface(IID_ICorDebugStringValue, (LPVOID*) &pStringValue));

    ULONG32 cchValue;
    IfFailRet(pStringValue->GetLength(&cchValue));
    cchValue++;         // Allocate one more for null terminator

    ArrayHolder<WCHAR> str = new WCHAR[cchValue];

    ULONG32 cchValueReturned;
    IfFailRet(pStringValue->GetString(
        cchValue,
        &cchValueReturned,
        str));

    output = to_utf8(str);

    return S_OK;
}

void EscapeString(string &s, char q = '\"')
{
    for (std::size_t i = 0; i < s.size(); ++i)
    {
        int count = 0;
        char c = s.at(i);
        switch (c)
        {
            case '\'':
                count = c != q ? 0 : 1;
                s.insert(i, count, '\\');
                break;
            case '\"':
                count = c != q ? 0 : 1;
                s.insert(i, count, '\\');
                break;
            case '\\':
                count = 1;
                s.insert(i, count, '\\');
                break;
            case '\0': count = 1; s.insert(i, count, '\\'); s[i + count] = '0'; break;
            case '\a': count = 1; s.insert(i, count, '\\'); s[i + count] = 'a'; break;
            case '\b': count = 1; s.insert(i, count, '\\'); s[i + count] = 'b'; break;
            case '\f': count = 1; s.insert(i, count, '\\'); s[i + count] = 'f'; break;
            case '\n': count = 1; s.insert(i, count, '\\'); s[i + count] = 'n'; break;
            case '\r': count = 1; s.insert(i, count, '\\'); s[i + count] = 'r'; break;
            case '\t': count = 1; s.insert(i, count, '\\'); s[i + count] = 't'; break;
            case '\v': count = 1; s.insert(i, count, '\\'); s[i + count] = 'v'; break;
        }
        i += count;
    }
}

HRESULT PrintValue(ICorDebugValue *pInputValue, string &output, bool escape)
{
    HRESULT Status;

    BOOL isNull = TRUE;
    ToRelease<ICorDebugValue> pValue;
    IfFailRet(DereferenceAndUnboxValue(pInputValue, &pValue, &isNull));

    if(isNull)
    {
        output = "null";
        return S_OK;
    }

    ULONG32 cbSize;
    IfFailRet(pValue->GetSize(&cbSize));
    ArrayHolder<BYTE> rgbValue = new (std::nothrow) BYTE[cbSize];
    if (rgbValue == nullptr)
    {
        return E_OUTOFMEMORY;
    }

    memset(rgbValue.GetPtr(), 0, cbSize * sizeof(BYTE));

    CorElementType corElemType;
    IfFailRet(pValue->GetType(&corElemType));
    if (corElemType == ELEMENT_TYPE_STRING)
    {
        string raw_str;
        IfFailRet(PrintStringValue(pValue, raw_str));

        if (!escape)
        {
            output = raw_str;
            return S_OK;
        }

        EscapeString(raw_str, '"');

        std::ostringstream ss;
        ss << "\"" << raw_str << "\"";
        output = ss.str();
        return S_OK;
    }

    if (corElemType == ELEMENT_TYPE_SZARRAY || corElemType == ELEMENT_TYPE_ARRAY)
    {
        return PrintArrayValue(pValue, output);
    }

    ToRelease<ICorDebugGenericValue> pGenericValue;
    IfFailRet(pValue->QueryInterface(IID_ICorDebugGenericValue, (LPVOID*) &pGenericValue));
    IfFailRet(pGenericValue->GetValue((LPVOID) &(rgbValue[0])));

    if(IsEnum(pValue))
    {
        return PrintEnumValue(pValue, rgbValue, output);
    }

    std::ostringstream ss;

    switch (corElemType)
    {
    default:
        ss << "(Unhandled CorElementType: 0x" << std::hex << corElemType << ")";
        break;

    case ELEMENT_TYPE_PTR:
        ss << "<pointer>";
        break;

    case ELEMENT_TYPE_FNPTR:
        {
            CORDB_ADDRESS addr = 0;
            ToRelease<ICorDebugReferenceValue> pReferenceValue;
            if(SUCCEEDED(pValue->QueryInterface(IID_ICorDebugReferenceValue, (LPVOID*) &pReferenceValue)))
                pReferenceValue->GetValue(&addr);
            ss << "<function pointer 0x" << std::hex << addr << ">";
        }
        break;

    case ELEMENT_TYPE_VALUETYPE:
    case ELEMENT_TYPE_CLASS:
        {
            string typeName;
            TypePrinter::GetTypeOfValue(pValue, typeName);
            if (typeName == "decimal") // TODO: implement mechanism for printing custom type values
            {
                string val;
                PrintDecimalValue(pValue, val);
                ss << val;
            }
            else
            {
                ss << '{' << typeName << '}';
            }
        }
        break;

    case ELEMENT_TYPE_BOOLEAN:
        ss << (rgbValue[0] == 0 ? "false" : "true");
        break;

    case ELEMENT_TYPE_CHAR:
        {
            WCHAR wc = * (WCHAR *) &(rgbValue[0]);
            string printableVal = to_utf8(wc);
            if (!escape)
            {
                output = printableVal;
                return S_OK;
            }
            EscapeString(printableVal, '\'');
            ss << (unsigned int)wc << " '" << printableVal << "'";
        }
        break;

    case ELEMENT_TYPE_I1:
        ss << (int) *(char*) &(rgbValue[0]);
        break;

    case ELEMENT_TYPE_U1:
        ss << (unsigned int) *(unsigned char*) &(rgbValue[0]);
        break;

    case ELEMENT_TYPE_I2:
        ss << *(short*) &(rgbValue[0]);
        break;

    case ELEMENT_TYPE_U2:
        ss << *(unsigned short*) &(rgbValue[0]);
        break;

    case ELEMENT_TYPE_I:
        ss << *(int*) &(rgbValue[0]);
        break;

    case ELEMENT_TYPE_U:
        ss << *(unsigned int*) &(rgbValue[0]);
        break;

    case ELEMENT_TYPE_I4:
        ss << *(int*) &(rgbValue[0]);
        break;

    case ELEMENT_TYPE_U4:
        ss << *(unsigned int*) &(rgbValue[0]);
        break;

    case ELEMENT_TYPE_I8:
        ss << *(__int64*) &(rgbValue[0]);
        break;

    case ELEMENT_TYPE_U8:
        ss << *(unsigned __int64*) &(rgbValue[0]);
        break;

    case ELEMENT_TYPE_R4:
        ss << std::setprecision(8) << *(float*) &(rgbValue[0]);
        break;

    case ELEMENT_TYPE_R8:
        ss << std::setprecision(16) << *(double*) &(rgbValue[0]);
        break;

    case ELEMENT_TYPE_OBJECT:
        ss << "object";
        break;

        // TODO: The following corElementTypes are not yet implemented here.  Array
        // might be interesting to add, though the others may be of rather limited use:
        //
        // ELEMENT_TYPE_GENERICINST    = 0x15,     // GENERICINST <generic type> <argCnt> <arg1> ... <argn>
    }

    output = ss.str();
    return S_OK;
}

HRESULT PrintBasicValue(int typeId, const string &rawData, string &typeName, string &value)
{
    std::ostringstream ss;
    switch(typeId)
    {
        case SymbolReader::TypeCorValue:
            ss << "null";
            typeName = "object";
            break;
        case SymbolReader::TypeObject:
            ss << "null";
            typeName = "object";
            break;
        case SymbolReader::TypeBoolean:
            ss << (rawData[0] == 0 ? "false" : "true");
            typeName = "bool";
            break;
        case SymbolReader::TypeByte:
            ss << (unsigned int) *(unsigned char*) &(rawData[0]);
            typeName = "byte";
            break;
        case SymbolReader::TypeSByte:
            ss << (int) *(char*) &(rawData[0]);
            typeName = "sbyte";
            break;
        case SymbolReader::TypeChar:
            {
                WCHAR wc = * (WCHAR *) &(rawData[0]);
                string printableVal = to_utf8(wc);
                EscapeString(printableVal, '\'');
                ss << (unsigned int)wc << " '" << printableVal << "'";
                typeName = "char";
            }
            break;
        case SymbolReader::TypeDouble:
            ss << std::setprecision(16) << *(double*) &(rawData[0]);
            typeName = "double";
            break;
        case SymbolReader::TypeSingle:
            ss << std::setprecision(8) << *(float*) &(rawData[0]);
            typeName = "float";
            break;
        case SymbolReader::TypeInt32:
            ss << *(int*) &(rawData[0]);
            typeName = "int";
            break;
        case SymbolReader::TypeUInt32:
            ss << *(unsigned int*) &(rawData[0]);
            typeName = "uint";
            break;
        case SymbolReader::TypeInt64:
            ss << *(__int64*) &(rawData[0]);
            typeName = "long";
            break;
        case SymbolReader::TypeUInt64:
            typeName = "ulong";
            ss << *(unsigned __int64*) &(rawData[0]);
            break;
        case SymbolReader::TypeInt16:
            ss << *(short*) &(rawData[0]);
            typeName = "short";
            break;
        case SymbolReader::TypeUInt16:
            ss << *(unsigned short*) &(rawData[0]);
            typeName = "ushort";
            break;
        case SymbolReader::TypeIntPtr:
            ss << "0x" << std::hex << *(intptr_t*) &(rawData[0]);
            typeName = "IntPtr";
            break;
        case SymbolReader::TypeUIntPtr:
            ss << "0x" << std::hex << *(intptr_t*) &(rawData[0]);
            typeName = "UIntPtr";
            break;
        case SymbolReader::TypeDecimal:
            PrintDecimalValue(rawData, value);
            typeName = "decimal";
            return S_OK;
        case SymbolReader::TypeString:
            {
                string rawStr = rawData;
                EscapeString(rawStr, '"');
                ss << "\"" << rawStr << "\"";
            }
            break;
    }
    value = ss.str();
    return S_OK;
}

HRESULT MarshalValue(ICorDebugValue *pInputValue, int *typeId, void **data)
{
    HRESULT Status;

    BOOL isNull = TRUE;
    ToRelease<ICorDebugValue> pValue;
    IfFailRet(DereferenceAndUnboxValue(pInputValue, &pValue, &isNull));

    if (isNull)
    {
        *data = nullptr;
        *typeId = SymbolReader::TypeObject;
        return S_OK;
    }

    ULONG32 cbSize;
    IfFailRet(Status = pValue->GetSize(&cbSize));

    // TODO: potencially memory leaks..For example, SZARRAY
    ArrayHolder<BYTE> rgbValue = new (std::nothrow) BYTE[cbSize];
    if (rgbValue == nullptr)
    {
        return E_OUTOFMEMORY;
    }

    memset(rgbValue.GetPtr(), 0, cbSize * sizeof(BYTE));

    CorElementType corElemType;
    IfFailRet(pValue->GetType(&corElemType));

    if (corElemType == ELEMENT_TYPE_STRING)
    {
        string raw_str;
        IfFailRet(PrintStringValue(pValue, raw_str));

        if (!raw_str.empty())
        {
            *data = SymbolReader::AllocString(raw_str);
            if (*data == nullptr)
                return E_FAIL;
        }
        else
        {
            *data = nullptr;
        }

        *typeId = SymbolReader::TypeString;
        return S_OK;
    }

    if (corElemType == ELEMENT_TYPE_SZARRAY || corElemType == ELEMENT_TYPE_ARRAY)
    {
        pInputValue->AddRef();
        *data = pInputValue;
        *typeId = SymbolReader::TypeCorValue;
        return S_OK;
    }

    ToRelease<ICorDebugGenericValue> pGenericValue;
    IfFailRet(pValue->QueryInterface(IID_ICorDebugGenericValue, (LPVOID*) &pGenericValue));
    IfFailRet(pGenericValue->GetValue((LPVOID) &(rgbValue[0])));

    if (IsEnum(pValue))
    {
        return E_FAIL;
        // TODO: Support enums, return PrintEnumValue(pValue, rgbValue, output);
    }

    switch (corElemType)
    {
    default:
        return E_FAIL;

    case ELEMENT_TYPE_PTR:
        *typeId = SymbolReader::TypeIntPtr;
        break;

    case ELEMENT_TYPE_FNPTR:
        {
            CORDB_ADDRESS addr = 0;
            ToRelease<ICorDebugReferenceValue> pReferenceValue;
            if(SUCCEEDED(pValue->QueryInterface(IID_ICorDebugReferenceValue, (LPVOID*) &pReferenceValue)))
                pReferenceValue->GetValue(&addr);
            *(CORDB_ADDRESS*) &(rgbValue[0]) = addr;
            *typeId = SymbolReader::TypeIntPtr;
        }
        break;

    case ELEMENT_TYPE_VALUETYPE:
    case ELEMENT_TYPE_CLASS:
        {
            string typeName;
            TypePrinter::GetTypeOfValue(pValue, typeName);
            if (typeName != "decimal")
            {
                pInputValue->AddRef();
                *data = pInputValue;
                *typeId = SymbolReader::TypeCorValue;
                return S_OK;
            }
            *typeId = SymbolReader::TypeDecimal;
        }
        break;

    case ELEMENT_TYPE_BOOLEAN:
        *typeId = SymbolReader::TypeBoolean;
        break;

    case ELEMENT_TYPE_CHAR:
        *typeId = SymbolReader::TypeChar;
        break;

    case ELEMENT_TYPE_I1:
        *typeId = SymbolReader::TypeSByte;
        break;

    case ELEMENT_TYPE_U1:
        *typeId = SymbolReader::TypeByte;
        break;

    case ELEMENT_TYPE_I2:
        *typeId = SymbolReader::TypeInt16;
        break;

    case ELEMENT_TYPE_U2:
        *typeId = SymbolReader::TypeUInt16;
        break;

    case ELEMENT_TYPE_I:
        *typeId = SymbolReader::TypeIntPtr;
        break;

    case ELEMENT_TYPE_U:
        *typeId = SymbolReader::TypeUIntPtr;
        break;

    case ELEMENT_TYPE_I4:
        *typeId = SymbolReader::TypeInt32;
        break;

    case ELEMENT_TYPE_U4:
        *typeId = SymbolReader::TypeUInt32;
        break;

    case ELEMENT_TYPE_I8:
        *typeId = SymbolReader::TypeInt64;
        break;

    case ELEMENT_TYPE_U8:
        *typeId = SymbolReader::TypeUInt64;
        break;

    case ELEMENT_TYPE_R4:
        *typeId = SymbolReader::TypeSingle;
        break;

    case ELEMENT_TYPE_R8:
        *typeId = SymbolReader::TypeDouble;
        break;

    case ELEMENT_TYPE_OBJECT:
        return E_FAIL;

    }

    *data = SymbolReader::AllocBytes(cbSize);
    if (*data == nullptr)
    {
        return E_FAIL;
    }
    memmove(*data, &(rgbValue[0]), cbSize);
    return S_OK;
}

HRESULT PrintStringField(ICorDebugValue *pValue, const WCHAR *fieldName, string &output, ICorDebugType *pType)
{
    output = "<unknown>";
    HRESULT Status;

    if (!pValue || !fieldName)
        return E_FAIL;

    if (!pType) {
        ToRelease<ICorDebugValue2> pValue2;
        IfFailRet(pValue->QueryInterface(IID_ICorDebugValue2, (LPVOID *)&pValue2));
        IfFailRet(pValue2->GetExactType(&pType));
    }

    ToRelease<ICorDebugClass> pClass;
    ToRelease<ICorDebugModule> pModule;
    IfFailRet(pType->GetClass(&pClass));
    IfFailRet(pClass->GetModule(&pModule));

    mdTypeDef currentTypeDef;
    IfFailRet(pClass->GetToken(&currentTypeDef));

    ToRelease<IUnknown> pMDUnknown;
    ToRelease<IMetaDataImport> pMD;
    IfFailRet(pModule->GetMetaDataInterface(IID_IMetaDataImport, &pMDUnknown));
    IfFailRet(pMDUnknown->QueryInterface(IID_IMetaDataImport, (LPVOID*)&pMD));

    ULONG numMethods = 0;
    HCORENUM fEnum = NULL;
    mdFieldDef fieldDef = mdFieldDefNil;

    // TODO: This string for new walking though the tree function
    // pMD->EnumFields(&fEnum, currentTypeDef, rTokens, 2048, &numMethods);

    numMethods = 0;
    fEnum = NULL;
    IfFailRet(pMD->EnumFieldsWithName(&fEnum, currentTypeDef, fieldName, &fieldDef, 1, &numMethods));
    pMD->CloseEnum(fEnum);

    if (numMethods == 1)
    {
        ULONG nameLen = 0;
        DWORD fieldAttr = 0;
        WCHAR mdName[mdNameLen] = { 0 };
        PCCOR_SIGNATURE pSignatureBlob = nullptr;
        ULONG sigBlobLength = 0;
        UVCP_CONSTANT pRawValue = nullptr;
        ULONG rawValueLength = 0;
        if (SUCCEEDED(pMD->GetFieldProps(fieldDef,
            nullptr,
            mdName,
            _countof(mdName),
            &nameLen,
            &fieldAttr,
            &pSignatureBlob,
            &sigBlobLength,
            nullptr,
            &pRawValue,
            &rawValueLength)))
        {
            if (pValue != nullptr)
            {
                ToRelease<ICorDebugValue> pValueDeref;
                if (SUCCEEDED(DereferenceAndUnboxValue(pValue, &pValueDeref, nullptr)))
                {
                    ToRelease<ICorDebugObjectValue> pObjValue;
                    if (SUCCEEDED(pValueDeref->QueryInterface(IID_ICorDebugObjectValue, (LPVOID*)&pObjValue)))
                    {
                        ToRelease<ICorDebugValue> pFieldVal;
                        IfFailRet(pObjValue->GetFieldValue(pClass, fieldDef, &pFieldVal));
                        // Print with any visible attributes. 
                        // Example for check private attribute: if ((fieldAttr & fdPrivate) && (fieldAttr & fdFamANDAssem))
                        ToRelease<ICorDebugValue> pValueDerefStr;
                        if (SUCCEEDED(DereferenceAndUnboxValue(pFieldVal, &pValueDerefStr, nullptr)))
                        {
                            IfFailRet(PrintValue(pValueDerefStr, output, true));
                            return S_OK;
                        }
                    }
                }
            }
        }
    }

    string baseTypeName;
    ToRelease<ICorDebugType> pBaseType;
    if (SUCCEEDED(pType->GetBase(&pBaseType)) && pBaseType != NULL &&
        SUCCEEDED(TypePrinter::GetTypeOfValue(pBaseType, baseTypeName)))
    {
        if (baseTypeName == "System.Enum")
            return E_FAIL;

        if (baseTypeName != "System.Object" && baseTypeName != "System.ValueType")
            return PrintStringField(pValue, fieldName, output, pBaseType);
    }

    return E_FAIL;
}
