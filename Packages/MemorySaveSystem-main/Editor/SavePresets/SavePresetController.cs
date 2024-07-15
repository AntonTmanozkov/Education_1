using UnityEngine;

namespace MemorySaveSystem.Editor.SavePresets
{
    internal class SavePresetController
    {
        private SavePresetList _view;
        private SavePresetLogic _logic;
        public SavePresetController(SavePresetList owner)
        {
            _view = owner;
            _logic = new();

            _view.AddButton.clicked += OnAddClick;
            _view.RemoveButton.clicked += OnRemoveClick;
            _view.RenameButton.clicked += OnRenameClick;
            _view.ApplyButton.clicked += OnApplySaveClick;
            _view.CreateCopyButton.clicked += OnCreateCopyClick;
            _view.LoadCurrentButton.clicked += OnLoadSaveClick;
        }

        private void OnAddClick()
        {
            _logic.CreatePreset();
            _view.UpdatePresetList();
        }

        private void OnRemoveClick()
        {
            _logic.RemovePreset(_view.SelectedPreset);
            _view.UpdatePresetList();
        }

        private void OnRenameClick()
        {
            _logic.RenamePreset(_view.SelectedPreset, _view.NameField.text);
            _view.UpdatePresetList();
            _view.UpdateSelectedItemInfo();
        }

        private void OnCreateCopyClick()
        {
            _logic.CreateCopy(_view.SelectedPreset);
            _view.UpdatePresetList();
        }

        private void OnApplySaveClick()
        {
            _logic.ApplyPreset(_view.SelectedPreset);
            Debug.Log($"Save preset was owerwriten on {_view.SelectedPreset}");
        }

        private void OnLoadSaveClick()
        {
            _logic.LoadFromCurrentSaveIn(_view.SelectedPreset);
            Debug.Log($"{_view.SelectedPreset.name} was owerwrited");
        }
    }
}
