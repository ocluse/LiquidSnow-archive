using System.Collections.Generic;
using Thismaker.Core;
using Thismaker.Goro.Commands;

namespace Thismaker.Thoth.Manager.Wpf
{
    class TranslationEditorViewModel : BindableBase
    {
        private string _translationContent;
        private List<ManagedLocale> _locales;
        private ManagedLocale _selLocale;
        private bool? _dlgResult;

        public string TranslationContent
        {
            get => _translationContent;
            set => SetProperty(ref _translationContent, value);
        }

        public List<ManagedLocale> Locales
        {
            get => _locales;
            set => SetProperty(ref _locales, value);
        }

        public ManagedLocale SelectedLocale
        {
            get => _selLocale;
            set => SetProperty(ref _selLocale, value);
        }

        public bool? DialogResult
        {
            get => _dlgResult;
            set => SetProperty(ref _dlgResult, value);
        }

        private LocTranslation _translation;

        public LocTranslation Translation
        {
            get => _translation;
        }

        public TranslationEditorViewModel()
        {
            SaveCommand = new RelayCommand(OnSave, CanSave);
            CancelCommand = new RelayCommand(OnCancel);
        }

        public void Start(LocTranslation translation, IEnumerable<ManagedLocale> locales)
        {
            Locales = new List<ManagedLocale>(locales);

            _translation = new LocTranslation();

            if (translation != null)
            {
                _translation.Value = translation.Value;
                _translation.Locale = translation.Locale;

                TranslationContent = _translation.Value;
                SelectedLocale = Locales.Find(x => x.ID == _translation.Locale);
            }
        }

        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        bool CanSave()
        {
            return !string.IsNullOrEmpty(TranslationContent) && SelectedLocale != null;
        }

        void OnSave()
        {
            _translation.Value = TranslationContent;
            _translation.Locale = SelectedLocale.ID;
            DialogResult = true;
        }

        void OnCancel()
        {
            DialogResult = false;
        }
    }
}
