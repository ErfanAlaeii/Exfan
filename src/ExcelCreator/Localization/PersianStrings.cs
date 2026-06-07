namespace ExcelCreator.Localization;



/// <summary>تمام متن‌های رابط کاربری به فارسی.</summary>

public static class PersianStrings

{

    public const string AppName = "Exfan";

    public const string MainWindowTitle = "Exfan";

    public const string WelcomeTitle = "به Exfan خوش آمدید";

    public const string AboutClose = "بستن";
    public const string AboutWindowTitle = "درباره برنامه";
    public const string SavedTablesWindowTitle = "جداول ذخیره‌شده";
    public const string TableEditorWindowTitle = "ویرایش جدول";

    public const string TemplateActionWindowTitle = "انتخاب عملیات";

    public const string CreateTableWindowTitle = "ایجاد جدول جدید";

    public const string VersionFooterFormat = "نسخه {0} — Exfan";



    // صفحه اصلی

    public const string MainSubtitle = "یک کار را از پایین انتخاب کنید. ما یک فایل اکسل آماده برای شما می‌سازیم.";

    public const string SearchPlaceholder = "جستجوی الگوها…";

    public const string NoTemplatesFound = "هیچ الگویی پیدا نشد. فایل‌های JSON را در پوشه Templates قرار دهید.";

    public const string NoTemplatesFoundShort = "هیچ الگویی پیدا نشد";

    public const string MainFooter = "نسخه ۱.۰ — بدون نیاز به دانستن فرمول‌های اکسل.";

    // تقویم

    public const string CalendarSectionTitle = "نوع تقویم تاریخ";

    public const string CalendarGregorian = "میلادی (Gregorian)";

    public const string CalendarJalali = "شمسی — جلالی";

    public const string DateFormatHintGregorian = "نمونه تاریخ: ۲۰۲۶/۰۶/۰۲ (میلادی)";

    public const string DateFormatHintJalali = "نمونه تاریخ: ۱۴۰۵/۰۳/۱۲ (شمسی)";

    public const string ImageColumnHint = "روی «انتخاب فایل» کلیک کنید. تصاویر (JPG، PNG، …) و PDF پشتیبانی می‌شوند.";

    public const string ImagePickButton = "انتخاب فایل";
    public const string ImageChangeButton = "تغییر فایل";
    public const string ImageClearButton = "حذف فایل";
    public const string ImageNotSelected = "هنوز فایلی انتخاب نشده است.";
    public const string ImageFileMissing = "فایل یافت نشد.";
    public const string ImagePickerTitle = "انتخاب فایل";
    public const string InvalidImageValue = "سطر {0} — فایل در ستون «{1}» یافت نشد.";
    public const string InvalidMediaFormat = "سطر {0} — فرمت فایل در ستون «{1}» پشتیبانی نمی‌شود.";
    public const string ExcelMediaClickTooltip = "یک‌بار کلیک کنید تا فایل باز شود";
    public const string ExcelImageOpenLink = "مشاهده تصویر";
    public const string ExcelPdfOpenLink = "مشاهده PDF";



    public const string AddRow = "افزودن";

    public const string EditRow = "ویرایش";

    public const string AddRowDialogTitle = "افزودن سطر";

    public const string EditRowDialogTitle = "ویرایش سطر";

    public const string AddRowValidation = "حداقل یکی از فیلدها را پر کنید.";

    public const string DataEntryEmpty = "هنوز سطری اضافه نشده است. روی «افزودن» کلیک کنید.";

    public const string DeleteRow = "حذف";

    public const string RowTimestampColumnHeader = "زمان ثبت";

    public const string Cancel = "انصراف";

    public const string Back = "قبلی";

    public const string Next = "بعدی";



    public const string Step3Title = "ورود داده";

    public const string Step3Subtitle = "سطرهای گزارش را با دکمه «افزودن» وارد کنید.";



    public const string SaveDialogTitle = "ذخیره فایل اکسل";

    public const string SaveDialogFilter = "فایل اکسل (*.xlsx)|*.xlsx";

    public const string CreateFileError = "امکان ساخت فایل وجود نداشت";



    // اعتبارسنجی

    public const string ValidationErrorTitle = "انتخاب نامعتبر";

    public const string ValidationErrorMessage = "لطفاً یکی از موارد زیر را انتخاب کنید: {0}";

    public const string TableNameRequired = "نام جدول الزامی است.";

    public const string TemplateHasNoColumns = "الگو فاقد ستون است.";

    public const string RowTooManyColumns = "سطر {0} تعداد ستون بیش از حد مجاز دارد.";

    public const string RowEmpty = "سطر {0} خالی است.";

    public const string TableTemplateMismatch = "جدول با الگوی فعلی سازگار نیست.";

    public const string TableVersionMismatch = "نسخه الگوی جدول ({0}) با نسخه فعلی ({1}) مطابقت ندارد.";

    public const string TableColumnMismatch = "ستون‌های ذخیره‌شده با الگوی فعلی مطابقت ندارند.";

    public const string InvalidDateValue = "سطر {0} — مقدار تاریخ در ستون «{1}» نامعتبر است.";

    public const string InvalidNumberValue = "سطر {0} — مقدار عددی در ستون «{1}» نامعتبر است.";

    public const string InvalidCurrencyValue = "سطر {0} — مقدار مبلغ در ستون «{1}» نامعتبر است.";

    public const string TemplateMissingSheet = "الگو فاقد برگه داده است.";



    // جداول ذخیره‌شده

    public const string ViewExistingTables = "مشاهده جداول موجود";

    public const string CreateNewTable = "ایجاد جدول جدید";

    public const string SelectTablePrompt = "لطفاً یک جدول را انتخاب کنید.";

    public const string DeleteTableConfirm = "آیا از حذف جدول «{0}» مطمئن هستید؟";

    public const string RowCountLabel = "تعداد سطر";

    public const string SaveChanges = "ذخیره تغییرات";

    public const string SaveChangesSuccess = "تغییرات با موفقیت ذخیره شد.";

    public const string ExportExcel = "خروجی اکسل";

    public const string UnsavedChangesPrompt = "تغییرات ذخیره نشده‌اند. آیا می‌خواهید قبل از خروج ذخیره کنید؟";

    public const string TableNameExists = "جدولی با این نام قبلاً ساخته شده است. نام دیگری انتخاب کنید.";

    public const string CreateTableTitle = "ایجاد جدول";

    public const string CreateTableButton = "ایجاد جدول";

    public const string CreateTableSuccess = "جدول با موفقیت ایجاد شد.";

    public const string CreateTableStepNameTitle = "نام جدول";

    public const string CreateTableStepNameSubtitle = "یک نام برای جدول جدید انتخاب کنید.";

    public const string CreateTableStep1Indicator = "مرحله ۱ از ۳ — نام جدول";

    public const string CreateTableStepCalendarTitle = "تقویم";

    public const string CreateTableStepCalendarSubtitle = "نوع تقویم تاریخ این جدول را انتخاب کنید.";

    public const string CreateTableStep2Indicator = "مرحله ۲ از ۳ — تقویم";

    public const string CreateTableStep3Indicator = "مرحله ۳ از ۳ — ورود داده و ایجاد";

    public const string CreateTableNameHint = "این نام برای شناسایی جدول در برنامه استفاده می‌شود.";

    public const string SavedTablesHeader = "جداول — {0}";

    public const string SavedTablesHint = "روی نام جدول کلیک کنید تا باز شود.";

    public const string NoSavedTables = "هنوز جدولی ذخیره نشده است.";

    public const string OpenTable = "باز کردن";

    public const string DeleteTable = "حذف";

    public const string BackToList = "بازگشت";

    public const string TemplateActionPrompt = "چه کاری می‌خواهید انجام دهید؟";

    public const string ViewExistingTablesHint = "جداولی که قبلاً برای این الگو ساخته‌اید را باز کنید و ویرایش کنید.";

    public const string CreateNewTableHint = "یک جدول تازه بسازید، سطرها را وارد کنید و ذخیره کنید.";

    public const string AddRowFormPrompt = "مقادیر این سطر را وارد کنید.";

    public const string Confirm = "تأیید";

    public const string CalculationsSectionTitle = "محاسبات";
    public const string CalculationsSectionHint = "روی هر دکمه کلیک کنید؛ برنامه بدون نیاز به فرمول اکسل، نتیجه را محاسبه می‌کند.";

    public const string CalculationSum = "جمع کل";
    public const string CalculationSumDescription = "جمع مقادیر عددی و مبلغی هر ستون";
    public const string CalculationSumSummary = "جمع ستون‌های عددی و مبلغی";

    public const string CalculationAverage = "میانگین";
    public const string CalculationAverageDescription = "میانگین مقادیر عددی و مبلغی هر ستون";
    public const string CalculationAverageSummary = "میانگین ستون‌های عددی و مبلغی";

    public const string CalculationMin = "کمترین مقدار";
    public const string CalculationMinDescription = "کمترین مقدار در هر ستون عددی یا مبلغی";

    public const string CalculationMax = "بیشترین مقدار";
    public const string CalculationMaxDescription = "بیشترین مقدار در هر ستون عددی یا مبلغی";

    public const string CalculationCountRows = "تعداد سطرها";
    public const string CalculationCountRowsDescription = "شمارش سطرهای واردشده در جدول";
    public const string CalculationCountRowsLabel = "تعداد سطر";

    public const string CalculationCountNumbers = "شمارش اعداد";
    public const string CalculationCountNumbersDescription = "شمارش مقادیر عددی معتبر در هر ستون";
    public const string CalculationCountNumbersSummary = "مجموع اعداد شمارش‌شده: {0}";

    public const string CalculationRowTotals = "جمع هر سطر";
    public const string CalculationRowTotalsDescription = "جمع ستون‌های عددی و مبلغی در هر سطر";
    public const string CalculationRowLabel = "سطر {0}";
    public const string CalculationRowTotalsSummary = "جمع کل سطرها: {0}";

    public const string CalculationResultDialogTitle = "نتیجه محاسبه";
    public const string CalculationResultClose = "بستن";
    public const string CalculationNotAvailable = "برای این محاسبه داده کافی وجود ندارد.";
    public const string CalculationNoNumericValues = "مقدار عددی معتبری برای محاسبه پیدا نشد.";
    public const string CalculationNoTimeValues = "مقدار زمانی معتبری پیدا نشد. از عدد ساعت (مثلاً ۸ یا ۱۷) یا فرمت ۸:۳۰ استفاده کنید.";
    public const string CalculationUnknownAction = "محاسبه درخواستی شناخته نشد.";
    public const string CalculationMissingColumns = "لطفاً ستون‌های مورد نظر را انتخاب کنید.";

    public const string CalculationPercentage = "درصد سهم";
    public const string CalculationPercentageDescription = "سهم هر سطر از جمع کل یک ستون عددی یا مبلغی";
    public const string CalculationPercentageSummary = "مجموع درصدها باید ۱۰۰٪ باشد";

    public const string CalculationColumnDifference = "تفاضل دو ستون";
    public const string CalculationColumnDifferenceDescription = "تفاضل مقدار دو ستون عددی یا مبلغی در هر سطر";
    public const string CalculationColumnDifferenceSummary = "ستون اول منهای ستون دوم";

    public const string CalculationColumnCompare = "مقایسه دو ستون";
    public const string CalculationColumnCompareDescription = "مقایسه دو ستون عددی در هر سطر (کم / کافی)";
    public const string CalculationCompareLessOrEqual = "کم";
    public const string CalculationCompareGreater = "کافی";

    public const string CalculationTimeDifference = "اختلاف زمان";
    public const string CalculationTimeDifferenceDescription = "مدت زمان بین دو ستون ساعت (مثلاً ورود و خروج)";
    public const string CalculationTimeDifferenceSummary = "ستون دوم منهای ستون اول";

    public const string CalculationColumnPickerTitle = "انتخاب ستون";
    public const string CalculationColumnPickerPrompt = "ستون‌های مورد نظر برای محاسبه را انتخاب کنید.";
    public const string CalculationPrimaryColumnLabel = "ستون اول";
    public const string CalculationSecondaryColumnLabel = "ستون دوم";

}

