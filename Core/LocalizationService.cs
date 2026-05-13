using System.ComponentModel;
using System.Windows;

namespace QuickPOS.Core;

public enum AppLanguage { French, Arabic }

public class LocalizationService : INotifyPropertyChanged
{
    public static LocalizationService Instance { get; } = new();

    private AppLanguage _language = AppLanguage.French;

    public AppLanguage Language
    {
        get => _language;
        set
        {
            if (_language == value) return;
            _language = value;
            OnPropertyChanged(nameof(Language));
            OnPropertyChanged(nameof(FlowDirection));
            OnPropertyChanged(nameof(IsArabic));
            OnPropertyChanged(string.Empty);
        }
    }

    public FlowDirection FlowDirection =>
        _language == AppLanguage.Arabic ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

    public bool IsArabic => _language == AppLanguage.Arabic;

    // ── LoginWindow ────────────────────────────────────────────────────────────
    /// <summary>LoginWindow · Branding panel — label above the default credentials card</summary>
    public string DefaultCreds   => T("IDENTIFIANTS PAR D\u00c9FAUT",     "\u0628\u064a\u0627\u0646\u0627\u062a \u0627\u0644\u0627\u0639\u062a\u0645\u0627\u062f \u0627\u0644\u0627\u0641\u062a\u0631\u0627\u0636\u064a\u0629");
    /// <summary>LoginWindow · Form panel — main heading ("Welcome back")</summary>
    public string WelcomeBack    => T("Bienvenue",                         "\u0645\u0631\u062d\u0628\u0627\u064b \u0628\u0643");
    /// <summary>LoginWindow · Form panel — subtitle below the heading</summary>
    public string SignInSubtitle => T("Connectez-vous \u00e0 votre compte", "\u062a\u0633\u062c\u064a\u0644 \u0627\u0644\u062f\u062e\u0648\u0644 \u0625\u0644\u0649 \u062d\u0633\u0627\u0628\u0643");
    /// <summary>LoginWindow · Form panel — label above the username TextBox</summary>
    public string LabelUsername  => T("NOM D'UTILISATEUR",                 "\u0627\u0633\u0645 \u0627\u0644\u0645\u0633\u062a\u062e\u062f\u0645");
    /// <summary>LoginWindow · Form panel — label above the password field</summary>
    public string LabelPassword  => T("MOT DE PASSE",                      "\u0643\u0644\u0645\u0629 \u0627\u0644\u0645\u0631\u0648\u0631");
    /// <summary>LoginWindow · Form panel — Sign In button (normal state)</summary>
    public string BtnSignIn      => T("Se connecter",                      "\u062a\u0633\u062c\u064a\u0644 \u0627\u0644\u062f\u062e\u0648\u0644");
    /// <summary>LoginWindow · Form panel — Sign In button (loading/busy state)</summary>
    public string BtnSigningIn   => T("Connexion...",                      "\u062c\u0627\u0631\u064c \u0627\u0644\u062f\u062e\u0648\u0644...");
    /// <summary>LoginWindow · Form panel — password visibility toggle (show label)</summary>
    public string ShowPassword   => T("Afficher",                          "\u0625\u0638\u0647\u0627\u0631");
    /// <summary>LoginWindow · Form panel — password visibility toggle (hide label)</summary>
    public string HidePassword   => T("Masquer",                           "\u0625\u062e\u0641\u0627\u0621");
    /// <summary>LoginWindow · Form panel — "Remember me" CheckBox content</summary>
    public string RememberMe     => T("Se souvenir de moi",                "\u062a\u0630\u0643\u0631\u0646\u064a");

    // ── MainWindow · Side Navigation ───────────────────────────────────────────
    /// <summary>MainWindow · Sidebar — sub-logo subtitle (POS manager label)</summary>
    public string NavPosManager  => T("Gestion PDV",          "\u0625\u062f\u0627\u0631\u0629 \u0646\u0642\u0637\u0629 \u0627\u0644\u0628\u064a\u0639");
    /// <summary>MainWindow · Sidebar — nav item text + tooltip for Selling (index 0)</summary>
    public string NavPosTerminal => T("Terminal PDV",          "\u0646\u0642\u0637\u0629 \u0627\u0644\u0628\u064a\u0639");
    /// <summary>MainWindow · Sidebar — nav item text + tooltip for Products (index 1)</summary>
    public string NavProducts    => T("Produits & Services",   "\u0627\u0644\u0645\u0646\u062a\u062c\u0627\u062a \u0648\u0627\u0644\u062e\u062f\u0645\u0627\u062a");
    /// <summary>MainWindow · Sidebar — nav item text + tooltip for Inventory (index 5)</summary>
    public string NavInventory   => T("Stock",            "\u0627\u0644\u0645\u062e\u0632\u0648\u0646");
    /// <summary>MainWindow · Sidebar — nav item text + tooltip for Dashboard (index 2)</summary>
    public string NavDashboard   => T("Tableau de bord",       "\u0644\u0648\u062d\u0629 \u0627\u0644\u062a\u062d\u0643\u0645");
    /// <summary>MainWindow · Sidebar — nav item text + tooltip for Daily Close (index 4)</summary>
    public string NavDailyClose  => T("Cl\u00f4ture journ.",  "\u0627\u0644\u0625\u063a\u0644\u0627\u0642 \u0627\u0644\u064a\u0648\u0645\u064a");
    /// <summary>MainWindow · Sidebar — nav item text + tooltip for Users (index 3)</summary>
    public string NavUsers       => T("Utilisateurs",          "\u0627\u0644\u0645\u0633\u062a\u062e\u062f\u0645\u0648\u0646");
    /// <summary>MainWindow · Sidebar — small label above the language switcher radio buttons</summary>
    public string LangLabel      => T("Langue",                "\u0627\u0644\u0644\u063a\u0629");
    /// <summary>MainWindow · Top bar — logout Button (top-right corner)</summary>
    public string BtnLogout      => T("D\u00e9connexion",      "\u062a\u0633\u062c\u064a\u0644 \u0627\u0644\u062e\u0631\u0648\u062c");

    // ── MainWindow · Page titles (bound to CurrentPageTitle in MainViewModel) ──
    /// <summary>MainViewModel · CurrentPageTitle — shown in top bar when Selling view is active</summary>
    public string PageTitleSelling    => T("Terminal PDV",              "\u0646\u0642\u0637\u0629 \u0627\u0644\u0628\u064a\u0639");
    /// <summary>MainViewModel · CurrentPageTitle — shown in top bar when Products view is active</summary>
    public string PageTitleProducts   => T("Produits & Services",       "\u0627\u0644\u0645\u0646\u062a\u062c\u0627\u062a \u0648\u0627\u0644\u062e\u062f\u0645\u0627\u062a");
    /// <summary>MainViewModel · CurrentPageTitle — shown in top bar when Dashboard view is active</summary>
    public string PageTitleDashboard  => T("Tableau de bord financier", "\u0644\u0648\u062d\u0629 \u0627\u0644\u062a\u062d\u0643\u0645 \u0627\u0644\u0645\u0627\u0644\u064a\u0629");
    /// <summary>MainViewModel · CurrentPageTitle — shown in top bar when Users view is active</summary>
    public string PageTitleUsers      => T("Utilisateurs & R\u00f4les", "\u0627\u0644\u0645\u0633\u062a\u062e\u062f\u0645\u0648\u0646 \u0648\u0627\u0644\u0623\u062f\u0648\u0627\u0631");
    /// <summary>MainViewModel · CurrentPageTitle — shown in top bar when Daily Close view is active</summary>
    public string PageTitleDailyClose => T("Cl\u00f4ture journ.",       "\u0627\u0644\u0625\u063a\u0644\u0627\u0642 \u0627\u0644\u064a\u0648\u0645\u064a");
    /// <summary>MainViewModel · CurrentPageTitle — shown in top bar when Inventory view is active</summary>
    public string PageTitleInventory  => T("Inventaire",                "\u0627\u0644\u0645\u062e\u0632\u0648\u0646");

    // ── ProductsView ───────────────────────────────────────────────────────────
    /// <summary>ProductsView · Stats bento — "TOTAL SKU" stat card label</summary>
    public string TotalSku          => T("TOTAL SKU",                    "\u0625\u062c\u0645\u0627\u0644\u064a SKU");
    /// <summary>ProductsView · Stats bento — unit suffix next to item count</summary>
    public string Items             => T("articles",                     "\u0639\u0646\u0627\u0635\u0631");
    /// <summary>ProductsView · Stats bento — "CATEGORIES" stat card label</summary>
    public string Categories        => T("CAT\u00c9GORIES",              "\u0627\u0644\u0641\u0626\u0627\u062a");
    /// <summary>ProductsView · Stats bento — active-items sub-label</summary>
    public string Active            => T("actives",                      "\u0646\u0634\u0637\u0629");
    /// <summary>ProductsView · Stats bento — "LOW STOCK" stat card label</summary>
    public string LowStock          => T("STOCK FAIBLE",                 "\u0645\u062e\u0632\u0648\u0646 \u0645\u0646\u062e\u0641\u0636");
    /// <summary>ProductsView · Tab strip — "PRODUCTS" tab header</summary>
    public string TabProducts       => T("PRODUITS",                     "\u0627\u0644\u0645\u0646\u062a\u062c\u0627\u062a");
    /// <summary>ProductsView · Tab strip — "CATEGORIES" tab header</summary>
    public string TabCategories     => T("CAT\u00c9GORIES",              "\u0627\u0644\u0641\u0626\u0627\u062a");
    /// <summary>ProductsView · Products tab — "+ Add Product" primary action button</summary>
    public string AddProduct        => T("+ Ajouter Produit",            "+ \u0625\u0636\u0627\u0641\u0629 \u0645\u0646\u062a\u062c");
    /// <summary>ProductsView · Categories tab — "+ Add Category" primary action button</summary>
    public string AddCategory       => T("+ Ajouter Cat\u00e9gorie",     "+ \u0625\u0636\u0627\u0641\u0629 \u0641\u0626\u0629");
    /// <summary>ProductsView · Products DataGrid — "ITEM NAME" column header</summary>
    public string ColItemName       => T("NOM DE L'ARTICLE",             "\u0627\u0633\u0645 \u0627\u0644\u0635\u0646\u0641");
    /// <summary>ProductsView · Products DataGrid — "CATEGORY" column header</summary>
    public string ColCategory       => T("CAT\u00c9GORIE",               "\u0627\u0644\u0641\u0626\u0629");
    /// <summary>ProductsView · Products DataGrid — "WHOLESALE" price column header</summary>
    public string ColWholesale      => T("GROS",                         "\u0627\u0644\u062c\u0645\u0644\u0629");
    /// <summary>ProductsView · Products DataGrid — "RETAIL" price column header</summary>
    public string ColRetail         => T("D\u00c9TAIL",                  "\u0627\u0644\u062a\u062c\u0632\u0626\u0629");
    /// <summary>ProductsView · Products DataGrid — "MARGIN" column header</summary>
    public string ColMargin         => T("MARGE",                        "\u0627\u0644\u0647\u0627\u0645\u0634");
    /// <summary>ProductsView · Products DataGrid — "ACTIONS" column header</summary>
    public string ColActions        => T("ACTIONS",                      "\u0627\u0644\u0625\u062c\u0631\u0627\u0621\u0627\u062a");
    /// <summary>ProductsView · Categories DataGrid — "TYPE" column header</summary>
    public string ColType           => T("TYPE",                         "\u0627\u0644\u0646\u0648\u0639");
    /// <summary>ProductsView · Categories DataGrid — "PRODUCTS" count column header</summary>
    public string ColProductCount   => T("PRODUITS",                     "\u0627\u0644\u0645\u0646\u062a\u062c\u0627\u062a");
    /// <summary>ProductsView · Table footer — "SHOWING" prefix before item count</summary>
    public string Showing           => T("AFFICHAGE",                    "\u0639\u0631\u0636");
    /// <summary>ProductsView · Table footer — "RESULTS" suffix after item count</summary>
    public string Results           => T("R\u00c9SULTATS",               "\u0646\u062a\u0627\u0626\u062c");
    /// <summary>ProductsView · Edit product modal — modal title</summary>
    public string EditItem          => T("Modifier l'article",           "\u062a\u0639\u062f\u064a\u0644 \u0627\u0644\u0635\u0646\u0641");
    /// <summary>ProductsView · Edit category modal — modal title</summary>
    public string CategoryDetails   => T("D\u00e9tails cat\u00e9gorie",  "\u062a\u0641\u0627\u0635\u064a\u0644 \u0627\u0644\u0641\u0626\u0629");
    /// <summary>ProductsView · Edit modals — primary "Save Changes" button</summary>
    public string SaveChanges       => T("Enregistrer",                  "\u062d\u0641\u0638 \u0627\u0644\u062a\u063a\u064a\u064a\u0631\u0627\u062a");
    /// <summary>ProductsView · Edit modals — secondary "Cancel" button</summary>
    public string Cancel            => T("Annuler",                      "\u0625\u0644\u063a\u0627\u0621");
    /// <summary>ProductsView · Delete confirmation dialog — dialog title</summary>
    public string ConfirmDeletion   => T("Confirmer la suppression",     "\u062a\u0623\u0643\u064a\u062f \u0627\u0644\u062d\u0630\u0641");
    /// <summary>ProductsView · Delete product dialog — warning body text</summary>
    public string DeleteWarning     => T("\u00cates-vous s\u00fbr de vouloir supprimer cet article ?", "\u0647\u0644 \u0623\u0646\u062a \u0645\u062a\u0623\u0643\u062f \u0645\u0646 \u062d\u0630\u0641 \u0647\u0630\u0627 \u0627\u0644\u0639\u0646\u0635\u0631\u061f");
    /// <summary>ProductsView · Delete category dialog — dialog title</summary>
    public string DeleteCategory    => T("Supprimer la cat\u00e9gorie",  "\u062d\u0630\u0641 \u0627\u0644\u0641\u0626\u0629");
    /// <summary>ProductsView · Delete category dialog — warning body text</summary>
    public string DeleteCatWarning  => T("Les produits devront \u00eatre r\u00e9affect\u00e9s.", "\u0633\u062a\u062d\u062a\u0627\u062c \u0627\u0644\u0645\u0646\u062a\u062c\u0627\u062a \u0625\u0644\u0649 \u0625\u0639\u0627\u062f\u0629 \u062a\u062e\u0635\u064a\u0635.");
    /// <summary>ProductsView · Delete dialogs — destructive "Delete" confirm button</summary>
    public string Delete            => T("Supprimer",                    "\u062d\u0630\u0641");
    /// <summary>ProductsView · Edit product form — "ITEM NAME" field label</summary>
    public string FieldItemName     => T("NOM DE L'ARTICLE",             "\u0627\u0633\u0645 \u0627\u0644\u0635\u0646\u0641");
    /// <summary>ProductsView · Edit product form — "SKU" field label</summary>
    public string FieldSku          => T("SKU",                          "\u0631\u0645\u0632 SKU");
    /// <summary>ProductsView · Edit product form — "WHOLESALE price" field label</summary>
    public string FieldWholesale    => T("GROS",                         "\u0633\u0639\u0631 \u0627\u0644\u062c\u0645\u0644\u0629");
    /// <summary>ProductsView · Edit product form — "RETAIL price" field label</summary>
    public string FieldRetail       => T("D\u00c9TAIL",                  "\u0633\u0639\u0631 \u0627\u0644\u062a\u062c\u0632\u0626\u0629");
    /// <summary>ProductsView · Edit product form — "STOCK QUANTITY" field label</summary>
    public string FieldStock        => T("QUANTIT\u00c9 EN STOCK",       "\u0627\u0644\u0643\u0645\u064a\u0629 \u0641\u064a \u0627\u0644\u0645\u062e\u0632\u0648\u0646");
    /// <summary>ProductsView · Edit product form — "It's a service" toggle label</summary>
    public string FieldIsService    => T("C'est un service",             "\u0647\u0630\u0627 \u0645\u0646\u062a\u062c \u062e\u062f\u0645\u064a");
    /// <summary>ProductsView · Edit category form — "CATEGORY NAME" field label</summary>
    public string FieldCategoryName => T("NOM DE LA CAT\u00c9GORIE",     "\u0627\u0633\u0645 \u0627\u0644\u0641\u0626\u0629");
    /// <summary>ProductsView · Edit category form — "Service category" toggle label</summary>
    public string FieldIsServiceCat => T("Cat\u00e9gorie de services",   "\u0641\u0626\u0629 \u0627\u0644\u062e\u062f\u0645\u0627\u062a");
    /// <summary>ProductsView · TYPE column — badge text for physical products</summary>
    public string TypeProduct       => T("Produit",                      "\u0645\u0646\u062a\u062c");
    /// <summary>ProductsView · TYPE column — badge text for service items</summary>
    public string TypeService       => T("Service",                      "\u062e\u062f\u0645\u0629");

    // ── SellingView ────────────────────────────────────────────────────────────
    /// <summary>SellingView · Cart sidebar header — cart panel title</summary>
    public string ActiveCart         => T("Panier actif",          "\u0627\u0644\u0633\u0644\u0629 \u0627\u0644\u0646\u0634\u0637\u0629");
    /// <summary>SellingView · Category filter pills — first pill showing all products</summary>
    public string AllCategories      => T("Toutes",                "\u0627\u0644\u0643\u0644");
    /// <summary>SellingView · Cart item row — unit price suffix label ("unit price")</summary>
    public string ItemPrice          => T("Prix U",                "\u0633\u0639\u0631 .\u0648");
    /// <summary>SellingView · Cart checkout footer — total amount label</summary>
    public string TotalPrice         => T("Prix Total",            "\u0627\u0644\u0633\u0639\u0631 \u0627\u0644\u0623\u062c\u0645\u0627\u0644\u064a");
    /// <summary>SellingView · Cart checkout footer — "Clear" secondary button</summary>
    public string Clear              => T("Effacer",               "\u0645\u0633\u062d");
    /// <summary>SellingView · Cart checkout footer — "Payment" primary button</summary>
    public string Payment            => T("Paiement",              "\u062f\u0641\u0639");
    /// <summary>SellingView · Cart item — "left in stock" badge suffix (e.g. "3 left")</summary>
    public string StockLeft          => T("restant",               "\u0645\u062a\u0628\u0642\u064a");
    /// <summary>SellingView · Out-of-stock product card overlay — overlay text</summary>
    public string OutOfStock         => T("RUPTURE DE STOCK",      "\u0646\u0641\u062f \u0627\u0644\u0645\u062e\u0632\u0648\u0646");

    // ── PaymentDialog ──────────────────────────────────────────────────────────
    /// <summary>PaymentDialog · Header — dialog title</summary>
    public string PaymentTitle       => T("Paiement",              "\u062f\u0641\u0639");
    /// <summary>PaymentDialog · Total row — "TOTAL" label on the left</summary>
    public string Total              => T("TOTAL",                 "\u0627\u0644\u0645\u062c\u0645\u0648\u0639");
    /// <summary>PaymentDialog · Cash input — label above the client bill TextBox</summary>
    public string GivingAmount       => T("Montant remis",         "\u0627\u0644\u0645\u0628\u0644\u063a \u0627\u0644\u0645\u0639\u0637\u0649");
    /// <summary>PaymentDialog · Validation — red warning shown when amount is insufficient</summary>
    public string InsufficientAmount => T("Montant insuffisant",   "\u0645\u0628\u0644\u063a \u063a\u064a\u0631 \u0643\u0627\u0641\u064d");
    /// <summary>PaymentDialog · Change row — "Change" label on the left</summary>
    public string CashReturned       => T("Argent rembours\u00e9", "\u0627\u0644\u0645\u0628\u0644\u063a \u0627\u0644\u0645\u0633\u062a\u0631\u062f");
    /// <summary>PaymentDialog · Confirm button — primary action button at the bottom</summary>
    public string Confirm            => T("Confirmer",             "\u062a\u0623\u0643\u064a\u062f");

    // ── DailyCloseView ─────────────────────────────────────────────────────────
    /// <summary>DailyCloseView · Header — "REPORTING" badge chip above the page title</summary>
    public string DcReporting          => T("RAPPORTS",                             "\u0627\u0644\u062a\u0642\u0627\u0631\u064a\u0631");
    /// <summary>DailyCloseView · Header — "SESSION OPEN" status badge next to reporting chip</summary>
    public string DcStatusOpen         => T("SESSION OUVERTE",                      "\u062c\u0644\u0633\u0629 \u0645\u0641\u062a\u0648\u062d\u0629");
    /// <summary>DailyCloseView · Header — main page title ("Daily Close")</summary>
    public string DcTitle              => T("Cl\u00f4ture journali\u00e8re",        "\u0627\u0644\u0625\u063a\u0644\u0627\u0642 \u0627\u0644\u064a\u0648\u0645\u064a");
    /// <summary>DailyCloseView · Header — date Run suffix ("— Day summary")</summary>
    public string DcDateSummary        => T("\u2014 R\u00e9sum\u00e9 de la journ\u00e9e", "\u2014 \u0645\u0644\u062e\u0635 \u0627\u0644\u064a\u0648\u0645");
    /// <summary>DailyCloseView · Header — top-right "Print Report" primary button</summary>
    public string DcPrintReport        => T("Imprimer le rapport",                  "\u0637\u0628\u0627\u0639\u0629 \u0627\u0644\u062a\u0642\u0631\u064a\u0631");
    /// <summary>DailyCloseView · Summary bento — "TOTAL SALES" KPI card label</summary>
    public string DcTotalSales         => T("TOTAL VENTES",                         "\u0625\u062c\u0645\u0627\u0644\u064a \u0627\u0644\u0645\u0628\u064a\u0639\u0627\u062a");
    /// <summary>DailyCloseView · Summary bento — "vs yesterday" sub-label below total sales</summary>
    public string DcVsYesterday        => T("vs hier",                              "\u0645\u0642\u0627\u0631\u0646\u0629 \u0628\u0627\u0644\u0623\u0645\u0633");
    /// <summary>DailyCloseView · Summary bento — "CASH" KPI card label</summary>
    public string DcCash               => T("ESP\u00c8CES",                         "\u0646\u0642\u062f\u0627\u064b");
    /// <summary>DailyCloseView · Summary bento — sub-label under cash card ("manual reconciliation required")</summary>
    public string DcManualReconcile    => T("Rapprochement manuel requis",          "\u0645\u0637\u0627\u0628\u0642\u0629 \u064a\u062f\u0648\u064a\u0629 \u0645\u0637\u0644\u0648\u0628\u0629");
    /// <summary>DailyCloseView · Summary bento — "CARD" KPI card label</summary>
    public string DcCard               => T("CARTE",                                "\u0628\u0637\u0627\u0642\u0629");
    /// <summary>DailyCloseView · Summary bento — sub-label under card KPI ("auto-settled")</summary>
    public string DcAutoSettled        => T("R\u00e9gl\u00e9 automatiquement",      "\u062a\u0633\u0648\u064a\u0629 \u062a\u0644\u0642\u0627\u0626\u064a\u0629");
    /// <summary>DailyCloseView · Summary bento — "AVG TICKET" KPI card label</summary>
    public string DcAvgTicket          => T("TICKET MOYEN",                         "\u0645\u062a\u0648\u0633\u0637 \u0627\u0644\u0641\u0627\u062a\u0648\u0631\u0629");
    /// <summary>DailyCloseView · Summary bento — sub-label under avg ticket ("loyalty points used")</summary>
    public string DcPointsTotal        => T("points fid\u00e9lit\u00e9 utilis\u00e9s", "\u0646\u0642\u0627\u0637 \u0627\u0644\u0648\u0644\u0627\u0621 \u0627\u0644\u0645\u0633\u062a\u062e\u062f\u0645\u0629");
    /// <summary>DailyCloseView · Payment breakdown panel — section title</summary>
    public string DcPaymentBreakdown   => T("R\u00e9partition des paiements",       "\u062a\u0648\u0632\u064a\u0639 \u0627\u0644\u0645\u062f\u0641\u0648\u0639\u0627\u062a");
    /// <summary>DailyCloseView · Payment breakdown panel — subtitle ("settlement by payment method")</summary>
    public string DcSettlementByMethod => T("R\u00e8glement par mode de paiement",  "\u0627\u0644\u062a\u0633\u0648\u064a\u0629 \u062d\u0633\u0628 \u0637\u0631\u064a\u0642\u0629 \u0627\u0644\u062f\u0641\u0639");
    /// <summary>DailyCloseView · Payment breakdown panel — reconciled status chip title</summary>
    public string DcVaultReconciled    => T("Coffre rapproch\u00e9",                "\u062a\u0645\u062a \u0645\u0637\u0627\u0628\u0642\u0629 \u0627\u0644\u062e\u0632\u064a\u0646\u0629");
    /// <summary>DailyCloseView · Payment breakdown panel — reconciled status chip subtitle</summary>
    public string DcConfirmedByManager => T("Confirm\u00e9 par le responsable",     "\u0645\u0624\u0643\u062f \u0645\u0646 \u0627\u0644\u0645\u062f\u064a\u0631");
    /// <summary>DailyCloseView · Payment breakdown bars — "Credit Card" bar label</summary>
    public string DcCreditCard         => T("Carte de cr\u00e9dit",                 "\u0628\u0637\u0627\u0642\u0629 \u0627\u0626\u062a\u0645\u0627\u0646");
    /// <summary>DailyCloseView · Payment breakdown bars — "Cash" bar label</summary>
    public string DcCashLabel          => T("Esp\u00e8ces",                         "\u0646\u0642\u062f\u0627\u064b");
    /// <summary>DailyCloseView · Payment breakdown bars — "Loyalty Points" bar label</summary>
    public string DcLoyaltyPoints      => T("Points fid\u00e9lit\u00e9",            "\u0646\u0642\u0627\u0637 \u0627\u0644\u0648\u0644\u0627\u0621");
    /// <summary>DailyCloseView · Transaction ledger panel — section title</summary>
    public string DcRecentTransactions => T("Transactions r\u00e9centes",           "\u0627\u0644\u0645\u0639\u0627\u0645\u0644\u0627\u062a \u0627\u0644\u0623\u062e\u064a\u0631\u0629");
    /// <summary>DailyCloseView · Transaction ledger panel — "View all" link button</summary>
    public string DcViewAll            => T("Voir tout",                             "\u0639\u0631\u0636 \u0627\u0644\u0643\u0644");
    /// <summary>DailyCloseView · Transactions DataGrid — "TIME" column header</summary>
    public string DcColTime            => T("HEURE",                                "\u0627\u0644\u0648\u0642\u062a");
    /// <summary>DailyCloseView · Transactions DataGrid — "ID" column header</summary>
    public string DcColId              => T("ID",                                   "\u0627\u0644\u0645\u0639\u0631\u0641");
    /// <summary>DailyCloseView · Transactions DataGrid — "METHOD" column header</summary>
    public string DcColMethod          => T("MODE",                                 "\u0627\u0644\u0637\u0631\u064a\u0642\u0629");
    /// <summary>DailyCloseView · Transactions DataGrid — "TOTAL" column header</summary>
    public string DcColTotal           => T("TOTAL",                                "\u0627\u0644\u0645\u062c\u0645\u0648\u0639");
    /// <summary>DailyCloseView · Footer action zone — title ("Ready to close?")</summary>
    public string DcReadyTitle         => T("Pr\u00eat \u00e0 cl\u00f4turer ?",     "\u0647\u0644 \u0623\u0646\u062a \u0645\u0633\u062a\u0639\u062f \u0644\u0644\u0625\u063a\u0644\u0627\u0642\u061f");
    /// <summary>DailyCloseView · Footer action zone — subtitle ("All data verified and ready")</summary>
    public string DcReadySubtitle      => T("Toutes les donn\u00e9es sont v\u00e9rifi\u00e9es et pr\u00eates.", "\u062a\u0645 \u0627\u0644\u062a\u062d\u0642\u0642 \u0645\u0646 \u062c\u0645\u064a\u0639 \u0627\u0644\u0628\u064a\u0627\u0646\u0627\u062a \u0648\u0647\u064a \u062c\u0627\u0647\u0632\u0629.");
    /// <summary>DailyCloseView · Footer action zone — "Resync" secondary button</summary>
    public string DcResync             => T("Resynchroniser",                        "\u0625\u0639\u0627\u062f\u0629 \u0627\u0644\u0645\u0632\u0627\u0645\u0646\u0629");
    /// <summary>DailyCloseView · Footer action zone — "Finalize Close" primary button</summary>
    public string DcFinalizeClose      => T("Finaliser la cl\u00f4ture",             "\u0625\u062a\u0645\u0627\u0645 \u0627\u0625\u063a\u0644\u0627\u0642");
    /// <summary>DailyCloseView · Cash reconciliation modal — modal title</summary>
    public string DcCashReconciliation => T("Rapprochement des esp\u00e8ces",        "\u0645\u0637\u0627\u0628\u0642\u0629 \u0627\u0644\u0646\u0642\u062f\u064a\u0629");
    /// <summary>DailyCloseView · Cash reconciliation modal — system cash total row label</summary>
    public string DcSystemCashTotal    => T("Total syst\u00e8me (esp\u00e8ces)",     "\u0625\u062c\u0645\u0627\u0644\u064a \u0627\u0644\u0646\u0638\u0627\u0645 (\u0646\u0642\u062f\u0627\u064b)");
    /// <summary>DailyCloseView · Cash reconciliation modal — instruction label above the drawer-total entry field</summary>
    public string DcCountCurrency      => T("SAISIR LE TOTAL DU TIROIR",             "\u0623\u062f\u062e\u0644 \u0625\u062c\u0645\u0627\u0644\u064a \u062f\u0631\u062c \u0627\u0644\u0646\u0642\u062f\u064a\u0629");
    /// <summary>DailyCloseView · Cash reconciliation modal — manual count total row label</summary>
    public string DcManualCount        => T("Total comptage manuel",                 "\u0625\u062c\u0645\u0627\u0644\u064a \u0627\u0644\u0639\u062f \u0627\u0644\u064a\u062f\u0648\u064a");
    /// <summary>DailyCloseView · Cash reconciliation modal — discrepancy row label</summary>
    public string DcDiscrepancy        => T("\u00c9cart",                             "\u0627\u0644\u0641\u0627\u0631\u0642");
    /// <summary>DailyCloseView · Cash reconciliation modal — "Submit" primary button</summary>
    public string DcSubmit             => T("Soumettre",                              "\u0625\u0631\u0633\u0627\u0644");
    /// <summary>DailyCloseView · Legacy page title (used by MainViewModel.DailyCloseTitle)</summary>
    public string DailyCloseTitle      => T("Cl\u00f4ture journ.",                   "\u0627\u0644\u0625\u063a\u0644\u0627\u0642 \u0627\u0644\u064a\u0648\u0645\u064a");

    // ── DailyCloseView · History tab ────────────────────────────────────────────
    /// <summary>DailyCloseView · Tab strip — "Today" tab pill label</summary>
    public string DcTabToday           => T("Aujourd'hui",                           "\u0627\u0644\u064a\u0648\u0645");
    /// <summary>DailyCloseView · Tab strip — "History" tab pill label (admin only)</summary>
    public string DcTabHistory         => T("Historique",                            "\u0627\u0644\u0633\u062c\u0644 \u0627\u0644\u062a\u0627\u0631\u064a\u062e\u064a");
    /// <summary>DailyCloseView · History DataGrid — "DATE" column header</summary>
    public string DcHistoryColDate     => T("DATE",                                  "\u0627\u0644\u062a\u0627\u0631\u064a\u062e");
    /// <summary>DailyCloseView · History DataGrid — "SYS. CASH" column header</summary>
    public string DcHistoryColCash     => T("ESP\u00c8CES SYST\u00c8ME",            "\u0646\u0642\u062f\u064a\u0629 \u0627\u0644\u0646\u0638\u0627\u0645");
    /// <summary>DailyCloseView · History DataGrid — "MANUAL COUNT" column header</summary>
    public string DcHistoryColManual   => T("COMPTAGE MANUEL",                       "\u0627\u0644\u0639\u062f \u0627\u0644\u064a\u062f\u0648\u064a");
    /// <summary>DailyCloseView · History DataGrid — "DISCREPANCY" column header</summary>
    public string DcHistoryColDiff     => T("\u00c9CART",                            "\u0627\u0644\u0641\u0627\u0631\u0642");
    /// <summary>DailyCloseView · History DataGrid — "TOTAL" column header</summary>
    public string DcHistoryColTotal    => T("TOTAL",                                 "\u0627\u0644\u0645\u062c\u0645\u0648\u0639");
    /// <summary>DailyCloseView · History DataGrid — "CLOSED BY" column header</summary>
    public string DcHistoryColClosedBy => T("CLÔTUR\u00c9 PAR",                     "\u0623\u063a\u0644\u0642 \u0628\u0648\u0627\u0633\u0637\u0629");
    /// <summary>DailyCloseView · History tab — empty-state message when no records exist</summary>
    public string DcHistoryEmpty       => T("Aucune cl\u00f4ture enregistr\u00e9e", "\u0644\u0627 \u062a\u0648\u062c\u062f \u0625\u063a\u0644\u0627\u0642\u0627\u062a \u0645\u0633\u062c\u0644\u0629");
    /// <summary>DailyCloseView · History tab header — record-count unit suffix (e.g. "5 clôtures")</summary>
    public string DcHistoryRecords     => T("cl\u00f4tures",                         "\u0625\u063a\u0644\u0627\u0642");

    // ── UsersView ──────────────────────────────────────────────────────────────
    /// <summary>UsersView · Header — main page title ("Staff Directory")</summary>
    public string UsersStaffDirectory   => T("Annuaire du personnel",                "\u062f\u0644\u064a\u0644 \u0627\u0644\u0645\u0648\u0638\u0641\u064a\u0646");
    /// <summary>UsersView · Header — subtitle below the main title</summary>
    public string UsersStaffSubtitle    => T("G\u00e9rez votre \u00e9quipe et leurs acc\u00e8s", "\u0625\u062f\u0627\u0631\u0629 \u0641\u0631\u064a\u0642\u0643 \u0648\u0635\u0644\u0627\u062d\u064a\u0627\u062a\u0647\u0645");
    /// <summary>UsersView · Header — "Invite New Member" primary button (top-right)</summary>
    public string UsersInviteNew        => T("Ajouter un membre", "إضافة عضو");
    /// <summary>UsersView · Stats bento — "TOTAL STAFF" card label</summary>
    public string UsersTotalStaff       => T("TOTAL PERSONNEL",                      "\u0625\u062c\u0645\u0627\u0644\u064a \u0627\u0644\u0645\u0648\u0638\u0641\u064a\u0646");
    /// <summary>UsersView · Stats bento — "ACTIVE NOW" card label</summary>
    public string UsersActiveNow        => T("ACTIFS MAINTENANT",                    "\u0627\u0644\u0646\u0634\u0637\u0648\u0646 \u0627\u0644\u0622\u0646");
    /// <summary>UsersView · Stats bento — badge inside active-now card ("on the floor")</summary>
    public string UsersOnFloor          => T("sur le terrain",                       "\u0641\u064a \u0627\u0644\u0645\u064a\u062f\u0627\u0646");
    /// <summary>UsersView · Stats bento — "PENDING INVITES" card label</summary>
    public string UsersPendingInvites   => T("INVITATIONS EN ATTENTE",               "\u0627\u0644\u062f\u0639\u0648\u0627\u062a \u0627\u0644\u0645\u0639\u0644\u0642\u0629");
    /// <summary>UsersView · Stats bento — badge inside pending-invites card ("awaiting response")</summary>
    public string UsersAwaitingResponse => T("en attente de r\u00e9ponse",           "\u0641\u064a \u0627\u0646\u062a\u0638\u0627\u0631 \u0627\u0644\u0631\u062f");
    /// <summary>UsersView · Staff table header bar — section title ("Team Members")</summary>
    public string UsersTeamMembers      => T("Membres de l'\u00e9quipe",             "\u0623\u0639\u0636\u0627\u0621 \u0627\u0644\u0641\u0631\u064a\u0642");
    /// <summary>UsersView · Staff table footer — unit suffix after member count (e.g. "12 members")</summary>
    public string UsersStaffMembers     => T("membres",                              "\u0645\u0648\u0638\u0641");
    /// <summary>UsersView · Add/Edit form modal — modal title when adding a new user</summary>
    public string UsersAddTitle         => T("Ajouter un utilisateur",               "\u0625\u0636\u0627\u0641\u0629 \u0645\u0633\u062a\u062e\u062f\u0645");
    /// <summary>UsersView · Add/Edit form modal — "PERMISSIONS" section label above checkboxes</summary>
    public string UsersFieldPermissions => T("ACCÈS AUX MODULES",                   "\u0635\u0644\u0627\u062d\u064a\u0627\u062a \u0627\u0644\u0648\u0635\u0648\u0644");
    /// <summary>UsersView · Add/Edit form modal — modal title</summary>
    public string UsersFormTitle        => T("Modifier l'utilisateur",               "\u062a\u0639\u062f\u064a\u0644 \u0627\u0644\u0645\u0633\u062a\u062e\u062f\u0645");
    /// <summary>UsersView · Add/Edit form modal — "FULL NAME" field label</summary>
    public string UsersFieldFullName    => T("NOM COMPLET",                          "\u0627\u0644\u0627\u0633\u0645 \u0627\u0644\u0643\u0627\u0645\u0644");
    /// <summary>UsersView · Add/Edit form modal — "EMAIL" field label</summary>
    public string UsersFieldEmail       => T("EMAIL",                                "\u0627\u0644\u0628\u0631\u064a\u062f \u0627\u0644\u0625\u0644\u0643\u062a\u0631\u0648\u0646\u064a");
    /// <summary>UsersView · Add/Edit form modal — "ROLE" ComboBox label</summary>
    public string UsersFieldRole        => T("R\u00d4LE",                            "\u0627\u0644\u062f\u0648\u0631");
    /// <summary>UsersView · Add/Edit form modal — "STATUS" ComboBox label</summary>
    public string UsersFieldStatus      => T("STATUT",                               "\u0627\u0644\u062d\u0627\u0644\u0629");
    /// <summary>UsersView · Add/Edit form modal — "Save" primary button</summary>
    public string UsersSave             => T("Enregistrer",                          "\u062d\u0641\u0638");
    /// <summary>UsersView · Legacy page title (used by MainViewModel.UsersTitle)</summary>
    public string UsersTitle            => T("Utilisateurs & R\u00f4les",            "\u0627\u0644\u0645\u0633\u062a\u062e\u062f\u0645\u0648\u0646 \u0648\u0627\u0644\u0623\u062f\u0648\u0627\u0631");

    // ── DashboardView ──────────────────────────────────────────────────────────
    /// <summary>DashboardView · Date range filter bar — "From" label before the start DatePicker</summary>
    public string DashboardFrom             => T("Du",                               "\u0645\u0646");
    /// <summary>DashboardView · Date range filter bar — "To" label before the end DatePicker</summary>
    public string DashboardTo               => T("Au",                               "\u0625\u0644\u0649");
    /// <summary>DashboardView · Date range filter bar — "Today" preset button</summary>
    public string DashboardTodayPreset      => T("Aujourd'hui",                      "\u0627\u0644\u064a\u0648\u0645");
    /// <summary>DashboardView · Date range filter bar — "This Week" preset button</summary>
    public string DashboardThisWeekPreset   => T("Cette semaine",                    "\u0647\u0630\u0627 \u0627\u0644\u0623\u0633\u0628\u0648\u0639");
    /// <summary>DashboardView · Date range filter bar — "This Month" preset button</summary>
    public string DashboardThisMonthPreset  => T("Ce mois",                          "\u0647\u0630\u0627 \u0627\u0644\u0634\u0647\u0631");
    /// <summary>DashboardView · Date range filter bar — "This Year" preset button</summary>
    public string DashboardThisYearPreset   => T("Cette ann\u00e9e",                 "\u0647\u0630\u0647 \u0627\u0644\u0633\u0646\u0629");
    /// <summary>DashboardView · Hero KPI cards — "REVENUE" card label</summary>
    public string DashboardTotalRevenue     => T("CHIFFRE D'AFFAIRES",               "\u0625\u062c\u0645\u0627\u0644\u064a \u0627\u0644\u0625\u064a\u0631\u0627\u062f\u0627\u062a");
    /// <summary>DashboardView · Hero KPI cards — gross margin sub-label under revenue value</summary>
    public string DashboardMarginText       => T("marge brute",                      "\u0647\u0627\u0645\u0634 \u0627\u0644\u0631\u0628\u062d \u0627\u0644\u0625\u062c\u0645\u0627\u0644\u064a");
    /// <summary>DashboardView · Hero KPI cards — "TOTAL EXPENSES" card label</summary>
    public string DashboardTotalExpenses    => T("TOTAL D\u00c9PENSES",              "\u0625\u062c\u0645\u0627\u0644\u064a \u0627\u0644\u0645\u0635\u0627\u0631\u064a\u0641");
    /// <summary>DashboardView · Hero KPI cards — "NET PROFIT" card label (gradient card)</summary>
    public string DashboardNetProfit        => T("B\u00c9N\u00c9FICE NET",           "\u0635\u0627\u0641\u064a \u0627\u0644\u0631\u0628\u062d");
    /// <summary>DashboardView · Hero KPI cards — sub-label under net profit ("Revenue – Expenses")</summary>
    public string DashboardRevenueMinusExpenses => T("Revenus - D\u00e9penses",      "\u0627\u0644\u0625\u064a\u0631\u0627\u062f\u0627\u062a - \u0627\u0644\u0645\u0635\u0627\u0631\u064a\u0641");
    /// <summary>DashboardView · Monthly bar chart — chart title ("Revenue vs Expenses")</summary>
    public string DashboardRevenueVsExpenses    => T("Revenus vs D\u00e9penses",     "\u0627\u0644\u0625\u064a\u0631\u0627\u062f\u0627\u062a \u0645\u0642\u0627\u0628\u0644 \u0627\u0644\u0645\u0635\u0627\u0631\u064a\u0641");
    /// <summary>DashboardView · Monthly bar chart — chart subtitle ("Monthly breakdown")</summary>
    public string DashboardMonthlyBreakdown     => T("R\u00e9partition mensuelle",   "\u0627\u0644\u062a\u0648\u0632\u064a\u0639 \u0627\u0644\u0634\u0647\u0631\u064a");
    /// <summary>DashboardView · Monthly bar chart — legend label for revenue bar</summary>
    public string DashboardRevenueLabel     => T("REVENUS",                          "\u0627\u0644\u0625\u064a\u0631\u0627\u062f\u0627\u062a");
    /// <summary>DashboardView · Monthly bar chart — legend label for expenses bar</summary>
    public string DashboardExpensesLabel    => T("D\u00c9PENSES",                    "\u0627\u0644\u0645\u0635\u0627\u0631\u064a\u0641");
    /// <summary>DashboardView · Recent flow panel — panel title ("Recent Flow")</summary>
    public string DashboardRecentFlow       => T("Flux r\u00e9cents",                "\u0627\u0644\u062d\u0631\u0643\u0627\u062a \u0627\u0644\u0623\u062e\u064a\u0631\u0629");
    /// <summary>DashboardView · Recent flow panel — panel subtitle ("Latest transactions")</summary>
    public string DashboardLatestTransactions => T("Derni\u00e8res transactions",    "\u0622\u062e\u0631 \u0627\u0644\u0645\u0639\u0627\u0645\u0644\u0627\u062a");
    /// <summary>DashboardView · Secondary KPI row — "GROSS MARGIN" card label (also used for today's revenue card)</summary>
    public string DashboardGrossMargin      => T("MARGE BRUTE",                      "\u0627\u0644\u0647\u0627\u0645\u0634 \u0627\u0644\u0625\u062c\u0645\u0627\u0644\u064a");
    /// <summary>DashboardView · Secondary KPI row — transaction count unit suffix (e.g. "24 transactions")</summary>
    public string DashboardTransactions     => T("transactions",                     "\u0645\u0639\u0627\u0645\u0644\u0629");
    /// <summary>DashboardView · Secondary KPI row — COGS sub-label under gross margin card</summary>
    public string DashboardCogsText         => T("co\u00fbt des ventes",             "\u062a\u0643\u0644\u0641\u0629 \u0627\u0644\u0645\u0628\u064a\u0639\u0627\u062a");
    /// <summary>DashboardView · Hero expenses card — "COGS" prefix in the sub-text (e.g. "COGS 500.00")</summary>
    public string DashboardCogsLabel        => T("COGS",                             "\u062a.\u0645.\u0645.");
    /// <summary>DashboardView · Secondary KPI — "CATALOGUE" card label</summary>
    public string DashboardCatalogue        => T("CATALOGUE",                        "\u0627\u0644\u0643\u062a\u0627\u0644\u0648\u062c");
    /// <summary>DashboardView · Secondary KPI — category count unit suffix (e.g. "4 catégories")</summary>
    public string DashboardCategories       => T("cat\u00e9gories",                  "\u0641\u0626\u0629");
    /// <summary>DashboardView · Secondary KPI — "SERVICES" card label</summary>
    public string DashboardServices         => T("SERVICES",                         "\u0627\u0644\u062e\u062f\u0645\u0627\u062a");
    /// <summary>DashboardView · Secondary KPI — "service items" sub-label under services count</summary>
    public string DashboardServiceItems     => T("articles de service",              "\u0639\u0646\u0635\u0631 \u062e\u062f\u0645\u064a");
    /// <summary>DashboardView · Stock overview panel — section title</summary>
    public string DashboardStockOverview    => T("Aper\u00e7u du stock",             "\u0646\u0638\u0631\u0629 \u0639\u0627\u0645\u0629 \u0639\u0644\u0649 \u0627\u0644\u0645\u062e\u0632\u0648\u0646");
    /// <summary>DashboardView · Stock overview panel — "TOTAL VALUE" mini-stat label</summary>
    public string DashboardTotalValue       => T("VALEUR TOTALE",                    "\u0627\u0644\u0642\u064a\u0645\u0629 \u0627\u0644\u0625\u062c\u0645\u0627\u0644\u064a\u0629");
    /// <summary>DashboardView · Stock overview panel — "TOTAL UNITS" mini-stat label</summary>
    public string DashboardTotalUnits       => T("TOTAL UNIT\u00c9S",                "\u0625\u062c\u0645\u0627\u0644\u064a \u0627\u0644\u0648\u062d\u062f\u0627\u062a");
    /// <summary>DashboardView · Stock overview panel — "AVG UNIT COST" mini-stat label</summary>
    public string DashboardAvgUnitCost      => T("CO\u00dbT UNIT. MOY.",             "\u0645\u062a\u0648\u0633\u0637 \u0627\u0644\u062a\u0643\u0644\u0641\u0629");
    /// <summary>DashboardView · Low stock card — "out of stock" suffix after count</summary>
    public string DashboardOutOfStock       => T("en rupture de stock",              "\u0646\u0641\u062f \u0627\u0644\u0645\u062e\u0632\u0648\u0646");
    /// <summary>DashboardView · Today's sales card — card title</summary>
    public string DashboardTodaySales       => T("Ventes du jour",                   "\u0645\u0628\u064a\u0639\u0627\u062a \u0627\u0644\u064a\u0648\u0645");
    /// <summary>DashboardView · Today's sales card — transaction count unit suffix</summary>
    public string DashboardTxns             => T("Txns",                             "\u0645\u0639\u0627\u0645\u0644\u0629");
    /// <summary>DashboardView · Monthly bar chart tooltip — "Expenses:" label prefix</summary>
    public string DashboardExpensesTooltip  => T("D\u00e9penses :",                  "\u0627\u0644\u0645\u0635\u0627\u0631\u064a\u0641 :");
    /// <summary>DashboardView · Monthly bar chart tooltip — "Revenue:" label prefix</summary>
    public string DashboardRevenueTooltip   => T("Revenus :",                        "\u0627\u0644\u0625\u064a\u0631\u0627\u062f\u0627\u062a :");
    /// <summary>DashboardView · Legacy page title (used by MainViewModel.DashboardTitle)</summary>
    public string DashboardTitle            => T("Tableau de bord financier",        "\u0644\u0648\u062d\u0629 \u0627\u0644\u062a\u062d\u0643\u0645 \u0627\u0644\u0645\u0627\u0644\u064a\u0629");

    // ── InventoryBatchView + ReceiveStockDialog ────────────────────────────────
    /// <summary>InventoryBatchView / ReceiveStockDialog — DataGrid "PRODUCT" column header + form field label</summary>
    public string InvColProduct             => T("PRODUIT",                          "\u0627\u0644\u0645\u0646\u062a\u062c");
    /// <summary>InventoryBatchView · DataGrid — "RECEIVED DATE" column header</summary>
    public string InvColReceived            => T("DATE R\u00c9CEPTION",              "\u062a\u0627\u0631\u064a\u062e \u0627\u0644\u0627\u0633\u062a\u0644\u0627\u0645");
    /// <summary>InventoryBatchView · DataGrid — "REFERENCE" column header</summary>
    public string InvColReference           => T("R\u00c9F\u00c9RENCE",              "\u0627\u0644\u0645\u0631\u062c\u0639");
    /// <summary>InventoryBatchView · DataGrid — "UNIT COST" column header</summary>
    public string InvColUnitCost            => T("CO\u00dbT UNITAIRE",               "\u0627\u0644\u062a\u0643\u0644\u0641\u0629 \u0627\u0644\u0648\u062d\u062f\u0648\u064a\u0629");
    /// <summary>InventoryBatchView · DataGrid — "QTY RECEIVED" column header</summary>
    public string InvColQtyIn               => T("QT\u00c9 RE\u00c7UE",              "\u0627\u0644\u0643\u0645\u064a\u0629 \u0627\u0644\u0645\u0633\u062a\u0644\u0645\u0629");
    /// <summary>InventoryBatchView · DataGrid — "QTY SOLD" column header</summary>
    public string InvColQtySold             => T("QT\u00c9 VENDUE",                  "\u0627\u0644\u0643\u0645\u064a\u0629 \u0627\u0644\u0645\u0628\u0627\u0639\u0629");
    /// <summary>InventoryBatchView · DataGrid — "REMAINING" column header (with progress bar)</summary>
    public string InvColRemaining           => T("RESTANT",                          "\u0627\u0644\u0645\u062a\u0628\u0642\u064a");
    /// <summary>InventoryBatchView · Remaining cell — text shown when a batch is fully depleted</summary>
    public string InvDepleted               => T("\u00c9PUIS\u00c9",                 "\u0646\u0641\u062f");
    /// <summary>InventoryBatchView · DataGrid — "LAYER VALUE" column header (remaining stock value)</summary>
    public string InvColLayerValue          => T("VALEUR COUCHE",                    "\u0642\u064a\u0645\u0629 \u0627\u0644\u0637\u0628\u0642\u0629");
    /// <summary>InventoryBatchView · FAB + ReceiveStockDialog header — "Receive Stock" action label</summary>
    public string InvReceiveStock           => T("R\u00e9ceptionner",                "\u0627\u0633\u062a\u0644\u0627\u0645 \u0627\u0644\u0645\u062e\u0632\u0648\u0646");
    /// <summary>ReceiveStockDialog · Header — subtitle below the dialog title</summary>
    public string InvReceiveStockSubtitle   => T("Ajouter un lot de stock entrant",  "\u0625\u0636\u0627\u0641\u0629 \u062f\u0641\u0639\u0629 \u0645\u062e\u0632\u0648\u0646 \u0648\u0627\u0631\u062f\u0629");
    /// <summary>ReceiveStockDialog · Form — "QTY RECEIVED" field label</summary>
    public string InvFieldQtyReceived       => T("QT\u00c9 RE\u00c7UE",             "\u0627\u0644\u0643\u0645\u064a\u0629 \u0627\u0644\u0645\u0633\u062a\u0644\u0645\u0629");
    /// <summary>ReceiveStockDialog · Form — "PURCHASE PRICE" field label</summary>
    public string InvFieldPurchasePrice     => T("PRIX D'ACHAT",                     "\u0633\u0639\u0631 \u0627\u0644\u0634\u0631\u0627\u0621");
    /// <summary>ReceiveStockDialog · Form — "SELLING PRICE" field label</summary>
    public string InvFieldSellingPrice      => T("PRIX DE VENTE",                    "\u0633\u0639\u0631 \u0627\u0644\u0628\u064a\u0639");
    /// <summary>ReceiveStockDialog · Form — "COSTING METHOD" ComboBox label</summary>
    public string InvFieldCostingMethod     => T("M\u00c9THODE DE CO\u00dbT",        "\u0637\u0631\u064a\u0642\u0629 \u0627\u0644\u062a\u0643\u0644\u0641\u0629");
    /// <summary>ReceiveStockDialog · Costing method ComboBox — "Weighted Average" option</summary>
    public string CostingWeightedAverage    => T("Co\u00fbt moyen pond\u00e9r\u00e9", "\u0627\u0644\u0645\u062a\u0648\u0633\u0637 \u0627\u0644\u0645\u0631\u062c\u062d");
    /// <summary>ReceiveStockDialog · Costing method ComboBox — "FIFO" option</summary>
    public string CostingFifo               => T("FIFO", "FIFO -  أول دخول أول خروج");
    /// <summary>ReceiveStockDialog · Costing method ComboBox — "LIFO" option</summary>
    public string CostingLifo               => T("LIFO", "LIFO - أخر دخول أول خروج");
    /// <summary>ReceiveStockDialog · Form — "REFERENCE / PO#" field label</summary>
    public string InvFieldReference         => T("R\u00c9F\u00c9RENCE / N\u00b0 BON", "\u0627\u0644\u0645\u0631\u062c\u0639 / \u0631\u0642\u0645 \u0627\u0644\u0637\u0644\u0628\u064a\u0629");
    /// <summary>ReceiveStockDialog · Form actions — "Save Batch" primary button</summary>
    public string InvSaveBatch              => T("Enregistrer le lot",               "\u062d\u0641\u0638 \u0627\u0644\u062f\u0641\u0639\u0629");

    // ── Language picker (always rendered in the language's own script) ──────────
    /// <summary>MainWindow · Language switcher + LoginWindow — French radio button label</summary>
    public string LangFrench => "Fran\u00e7ais";
    /// <summary>MainWindow · Language switcher + LoginWindow — Arabic radio button label</summary>
    public string LangArabic => "\u0627\u0644\u0639\u0631\u0628\u064a\u0629";

    private string T(string fr, string ar) => _language == AppLanguage.Arabic ? ar : fr;

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
