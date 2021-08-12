﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using ReactiveUI;

namespace TemplatedDataGridDemo.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<ItemViewModel> _items;

        public ObservableCollection<ItemViewModel> Items
        {
            get => _items;
            set => this.RaiseAndSetIfChanged(ref _items, value);
        }

        public MainWindowViewModel()
        {
            _items = new ObservableCollection<ItemViewModel>();

            int totalItems = 10_000;
            bool enableRandom = false;
            int randomSize = 100;

            var rand = new Random();
            var items = new List<ItemViewModel>();

            for (var i = 0; i < totalItems; i++)
            {
                var item = new ItemViewModel()
                {
                    Column1 = $"Test {i}-1",
                    Column2 = $"Test {i}-2",
                    Column3 = $"Test {i}-3",
                    Column4 = $"Test {i}-4",
                    Column5 = $"Test {i}-5",
                    Margin = enableRandom ? new Thickness(0, rand.NextDouble() * randomSize, 0, rand.NextDouble() * randomSize) : new Thickness(0)
                };

                items.Add(item);
            }

            Items = new ObservableCollection<ItemViewModel>(items);
        }
    }
}
