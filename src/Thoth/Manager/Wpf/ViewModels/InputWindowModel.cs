using Thismaker.Core;
using Thismaker.Goro;

namespace Thismaker.Thoth.Manager.Wpf
{
    class InputWindowModel : BindableBase
    {
        private bool? _dlgResult;
        private string _inputString, _header;

        public bool? DialogResult
        {
            get => _dlgResult;
            set => SetProperty(ref _dlgResult, value);
        }

        public string InputString
        {
            get => _inputString;
            set => SetProperty(ref _inputString, value);
        }

        public string Header
        {
            get => _header;
            set => SetProperty(ref _header, value);
        }

        public RelayCommand OkCommand { get; set; }

        public RelayCommand CancelCommand { get; set; }

        public InputWindowModel()
        {
            OkCommand = new RelayCommand(OnOk, CanOk);
            CancelCommand = new RelayCommand(OnCancel);
        }

        private bool CanOk()
        {
            return !string.IsNullOrEmpty(InputString);
        }

        private void OnOk()
        {
            DialogResult = true;
        }

        private void OnCancel()
        {
            DialogResult = false;
        }
    }
}
