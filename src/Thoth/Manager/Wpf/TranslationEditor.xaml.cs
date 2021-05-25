using System.Collections.Generic;
using System.Windows;

namespace Thismaker.Thoth.Manager.Wpf
{
    /// <summary>
    /// Interaction logic for TranslationEditor.xaml
    /// </summary>
    public partial class TranslationEditor : Window
    {
        public TranslationEditor(IEnumerable<ManagedLocale> locales, LocTranslation locTranslation=null)
        {
            InitializeComponent();

            ((TranslationEditorViewModel)DataContext).Start(locTranslation, locales);
        }

        public LocTranslation Translation
        {
            get => ((TranslationEditorViewModel)DataContext).Translation;
        }
    }
}
