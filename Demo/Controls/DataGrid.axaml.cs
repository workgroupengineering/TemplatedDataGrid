﻿using System.Collections;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls.Primitives;

namespace Demo.Controls
{
    public class DataGrid : TemplatedControl
    {
        public static readonly StyledProperty<AvaloniaList<DataGridColumn>> ColumnsProperty = 
            AvaloniaProperty.Register<DataGrid, AvaloniaList<DataGridColumn>>(nameof(Columns), new AvaloniaList<DataGridColumn>());

        public static readonly StyledProperty<IEnumerable> ItemsProperty = 
            AvaloniaProperty.Register<DataGrid, IEnumerable>(nameof(Items));

        public static readonly StyledProperty<object?> SelectedItemProperty = 
            AvaloniaProperty.Register<DataGrid, object?>(nameof(SelectedItem));

        public AvaloniaList<DataGridColumn> Columns
        {
            get => GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }

        public IEnumerable Items
        {
            get => GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public object? SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }
    }
}