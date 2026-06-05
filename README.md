# سازنده اکسل خالق: عرفان (Excel Creator)

برنامه دسکتاپ برای مبتدیان: یک کار را انتخاب کنید، چند مرحله ساده را طی کنید، فایل `.xlsx` آماده دریافت کنید. **تمام رابط کاربری و الگوها به فارسی هستند** و چیدمان از راست به چپ (RTL) است.

## پیش‌نیازها

- ویندوز ۱۰ یا ۱۱
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- اختیاری: Visual Studio 2022 با workload **توسعه دسکتاپ .NET**

## ساخت و اجرا (توسعه‌دهندگان)

```powershell
cd c:\Users\P30LAPTOP\Desktop\excelcreator
dotnet restore
dotnet build
dotnet run --project src\ExcelCreator\ExcelCreator.csproj
```

یا فایل `ExcelCreator.sln` را در Visual Studio باز کنید و **F5** بزنید.

## تحویل به کاربران (نسخه سازمانی)

برای ساخت **نصب‌کننده ویندوز** و بسته قابل تحویل:

```powershell
.\scripts\build-installer.ps1
```

خروجی‌ها در پوشه `dist\`:

| فایل | کاربرد |
|------|--------|
| `installer\ExcelCreator-Setup-1.0.0-win-x64.exe` | نصب استاندارد — تحویل به کاربران |
| `ExcelCreator-1.0.0-win-x64-portable.zip` | نسخه پرتابل بدون نصب |
| `app\` | برنامه آماده اجرا |

راهنمای کامل IT: **[DEPLOYMENT.md](DEPLOYMENT.md)** (شامل SCCM/Intune و نصب خاموش)

## تست‌ها

```powershell
.\scripts\run-tests.ps1              # همه تست‌ها
.\scripts\run-tests.ps1 -UnitOnly    # فقط صحت (accuracy)
.\scripts\run-tests.ps1 -PerformanceOnly  # فقط کارایی
.\scripts\run-tests.ps1 -Coverage    # با گزارش پوشش کد
```

پروژه تست: `tests/ExcelCreator.Tests` — xUnit + FluentAssertions

| دسته | محتوا |
|------|--------|
| **صحت** | تقویم میلادی/شمسی، بارگذاری الگو، ساخت اکسل، فرمول موجودی |
| **کارایی** | سقف زمانی بارگذاری الگو (&lt;500ms)، ساخت کارپوشه (&lt;2–5s) |

## ویژگی‌های فارسی

- رابط WPF با **RTL** و فونت **Tahoma**
- فرهنگ برنامه: `fa-IR` (تاریخ و دیالوگ‌های سیستم)
- برگه‌های داده با جهت راست به چپ
- خروجی اکسل ساده: سرستون سفید، بدون برگه راهنما

## تقویم میلادی و شمسی (جلالی)

اپراتور می‌تواند **میلادی** یا **شمسی** را انتخاب کند:

- **ایجاد جدول (مرحله تقویم)** — انتخاب برای همان جدول
- **اکسل** — میلادی: سلول تاریخ واقعی با قالب `yyyy/mm/dd`؛ شمسی: متن با قالب `1405/03/12` و ارقام فارسی

الگوهای JSON همچنان تاریخ نمونه را به‌صورت میلادی ISO نگه می‌دارند (مثال `2026-06-02`); برنامه هنگام ساخت فایل تبدیل می‌کند.

ورودی‌های قابل قبول برای تاریخ شمسی: `1405/03/12` یا `۱۴۰۵/۰۳/۱۲`

## جریان کار

1. **صفحه اصلی** — انتخاب الگو
2. **انتخاب عمل** — مشاهده جداول موجود یا ایجاد جدول جدید
3. **ویرایش جدول** — افزودن / ویرایش / حذف سطرها؛ ذخیره یا خروجی اکسل

جداول در `%AppData%\Exfan\tables.json` ذخیره می‌شوند.

## افزودن الگوی جدید

فایلی در `src/ExcelCreator/Templates/` بسازید (متن‌ها را فارسی بنویسید):

```json
{
  "id": "my-template",
  "title": "گزارش من",
  "description": "توضیح کوتاه برای کاربر.",
  "category": "مالی",
  "icon": "📊",
  "defaultFileName": "گزارش_من",
  "workbook": {
    "sheets": [{
      "name": "داده",
      "features": ["freeze_header"],
      "columns": [
        { "header": "نام", "type": "text", "width": 20 }
      ]
    }]
  }
}
```

### انواع ستون

`text`, `number`, `date`, `currency`

### امکانات برگه

`freeze_header`, `auto_filter`, `table_style`, `alternate_rows`

### فرمول

`"formula": "=IF(C{row}<=D{row},\"کم\",\"کافی\")"` — `{row}` شماره سطر اکسل است.

## الگوی آماده

| الگو | کاربرد |
|------|--------|
| گزارش فعالیت پرسنل | نام، ساعت ورود، ساعت خروج، شرح کار |

## ساختار پروژه

```
src/ExcelCreator/
  Abstractions/              # ISavedTableRepository, IAppNavigator, ...
  Composition/ServiceRegistration.cs  # DI container
  Controls/                  # TableRowsEditor, CalendarSelectorControl
  Excel/                     # ExcelCellFormatter, SheetFeatureApplier
  Infrastructure/            # AppPaths, AtomicJsonStore, UiMetrics, ExcelTheme
  Localization/PersianStrings.cs
  Models/                    # SavedTable, TemplateDefinition, ColumnTypes
  Navigation/AppNavigator.cs
  Services/                  # repositories, ExcelExportFacade
  Validation/                # TableValidator, TemplateValidator
  Templates/*.json
```
