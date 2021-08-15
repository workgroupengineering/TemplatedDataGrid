﻿using System.Collections.Generic;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace TemplatedDataGrid.Primitives
{
    public class DataGridCellsPresenter : TemplatedControl
    {
        internal static readonly DirectProperty<DataGridCellsPresenter, AvaloniaList<DataGridColumn>?> ColumnsProperty =
            AvaloniaProperty.RegisterDirect<DataGridCellsPresenter, AvaloniaList<DataGridColumn>?>(
                nameof(Columns), 
                o => o.Columns, 
                (o, v) => o.Columns = v);

        private AvaloniaList<DataGridColumn>? _columns;
        private Grid? _root;
        private readonly List<Control> _rootChildren = new List<Control>();

        internal AvaloniaList<DataGridColumn>? Columns
        {
            get => _columns;
            set => SetAndRaise(ColumnsProperty, ref _columns, value);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _root = e.NameScope.Find<Grid>("PART_Root");

            InvalidateRoot();
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ColumnsProperty)
            {
                InvalidateRoot();
            }
        }

        private void InvalidateRoot()
        {
            if (_root is null)
            {
                return;
            }

            foreach (var child in _rootChildren)
            {
                _root.Children.Remove(child);
            }

            var columns = Columns;
            if (columns is null)
            {
                return;
            }
 
            // Generate ColumnDefinitions

            var columnDefinitions = new List<ColumnDefinition>();

            for (var i = 0; i < columns.Count; i++)
            {
                var column = columns[i];
                var isAutoWidth = column.Width == GridLength.Auto;

                var columnDefinition = new ColumnDefinition()
                {
                    [!ColumnDefinition.MinWidthProperty] = column[!DataGridColumn.MinWidthProperty],
                    [!ColumnDefinition.MaxWidthProperty] = column[!DataGridColumn.MaxWidthProperty]
                };

                if (isAutoWidth)
                {
                    columnDefinition.Width = column.Width;
                    columnDefinition.SetValue(DefinitionBase.SharedSizeGroupProperty, $"Column{i}");
                }
                else
                {
                    columnDefinition[!ColumnDefinition.WidthProperty] = column[!DataGridColumn.WidthProperty];
                }

                columnDefinitions.Add(columnDefinition);

                // Generate DataGridCell's

                if (column is DataGridTemplateColumn templateColumn)
                {
                    var cell = new DataGridCell()
                    {
                        [Grid.ColumnProperty] = columnDefinitions.Count - 1,
                        [!DataGridCell.ContentProperty] = this[!DataGridCellsPresenter.DataContextProperty],
                        [!DataGridCell.CellTemplateProperty] = templateColumn[!DataGridColumn.CellTemplateProperty]
                    };
                    _rootChildren.Add(cell);
                }
                else if (column is DataGridTextColumn textColumn)
                {
                    var cell = new DataGridCell()
                    {
                        [Grid.ColumnProperty] = columnDefinitions.Count - 1,
                        [!DataGridCell.ContentProperty] = this[!DataGridCellsPresenter.DataContextProperty],
                        [!DataGridCell.CellTemplateProperty] = textColumn[!DataGridColumn.CellTemplateProperty]
                    };
                    _rootChildren.Add(cell);
                }
                else if (column is DataGridCheckBoxColumn checkBoxColumn)
                {
                    var cell = new DataGridCell()
                    {
                        [Grid.ColumnProperty] = columnDefinitions.Count - 1,
                        [!DataGridCell.ContentProperty] = this[!DataGridCellsPresenter.DataContextProperty],
                        [!DataGridCell.CellTemplateProperty] = checkBoxColumn[!DataGridColumn.CellTemplateProperty]
                    };
                    _rootChildren.Add(cell);
                }
                else
                {
                    var cell = new DataGridCell()
                    {
                        [Grid.ColumnProperty] = columnDefinitions.Count - 1,
                        [!DataGridCell.ContentProperty] = this[!DataGridCellsPresenter.DataContextProperty],
                        [!DataGridCell.CellTemplateProperty] = column[!DataGridColumn.CellTemplateProperty]
                    };
                    _rootChildren.Add(cell);
                }

                if (i < columns.Count - 1)
                {
                    columnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Pixel)));
                }
            }

            _root.ColumnDefinitions.Clear();
            _root.ColumnDefinitions.AddRange(columnDefinitions);

            foreach (var child in _rootChildren)
            {
                _root.Children.Add(child);
            }
        }
    }
}
