using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
