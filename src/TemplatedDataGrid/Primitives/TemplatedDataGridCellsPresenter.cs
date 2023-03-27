﻿using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using TemplatedDataGrid.Internal;

namespace TemplatedDataGrid.Primitives
{
    public class TemplatedDataGridCellsPresenter : TemplatedControl
    {
        internal static readonly DirectProperty<TemplatedDataGridCellsPresenter, object?> SelectedItemProperty =
            AvaloniaProperty.RegisterDirect<TemplatedDataGridCellsPresenter, object?>(
                nameof(SelectedItem), 
                o => o.SelectedItem, 
                (o, v) => o.SelectedItem = v,
                defaultBindingMode: BindingMode.TwoWay);

        internal static readonly DirectProperty<TemplatedDataGridCellsPresenter, object?> SelectedCellProperty =
            AvaloniaProperty.RegisterDirect<TemplatedDataGridCellsPresenter, object?>(
                nameof(SelectedCell), 
                o => o.SelectedCell, 
                (o, v) => o.SelectedCell = v,
                defaultBindingMode: BindingMode.TwoWay);

        internal static readonly DirectProperty<TemplatedDataGridCellsPresenter, AvaloniaList<TemplatedDataGridColumn>?> ColumnsProperty =
            AvaloniaProperty.RegisterDirect<TemplatedDataGridCellsPresenter, AvaloniaList<TemplatedDataGridColumn>?>(
                nameof(Columns), 
                o => o.Columns, 
                (o, v) => o.Columns = v);

        internal static readonly DirectProperty<TemplatedDataGridCellsPresenter, AvaloniaList<TemplatedDataGridCell>> CellsProperty =
            AvaloniaProperty.RegisterDirect<TemplatedDataGridCellsPresenter, AvaloniaList<TemplatedDataGridCell>>(
                nameof(Cells), 
                o => o.Cells, 
                (o, v) => o.Cells = v);

        private object? _selectedItem;
        private object? _selectedCell;
        private AvaloniaList<TemplatedDataGridColumn>? _columns;
        private AvaloniaList<TemplatedDataGridCell> _cells = new ();
        private Grid? _root;
        private readonly List<Control> _rootChildren = new ();

        internal object? SelectedItem
        {
            get => _selectedItem;
            set => SetAndRaise(SelectedItemProperty, ref _selectedItem, value);
        }

        internal object? SelectedCell
        {
            get => _selectedCell;
            set => SetAndRaise(SelectedCellProperty, ref _selectedCell, value);
        }

        internal AvaloniaList<TemplatedDataGridColumn>? Columns
        {
            get => _columns;
            set => SetAndRaise(ColumnsProperty, ref _columns, value);
        }

        internal AvaloniaList<TemplatedDataGridCell> Cells
        {
            get => _cells;
            set => SetAndRaise(CellsProperty, ref _cells, value);
        }

        internal CompositeDisposable? RootDisposables { get; set; }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _root = e.NameScope.Find<Grid>("PART_Root");

            InvalidateRoot();
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
#if DEBUG
            Console.WriteLine($"[TemplatedDataGridCellsPresenter.Attached] {DataContext}");
#endif
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
#if DEBUG
            Console.WriteLine($"[TemplatedDataGridCellsPresenter.Detach] {DataContext}");
#endif
            Detach();
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == SelectedItemProperty)
            {
                // TODO:
            }

            if (change.Property == SelectedCellProperty)
            {
                // TODO:
            }

            if (change.Property == ColumnsProperty)
            {
                InvalidateRoot();
            }
        }

        private void InvalidateRoot()
        {
            Detach();
            Attach();
        }

        internal void Attach()
        {
            if (_root is null)
            {
                return;
            }

            RootDisposables = new CompositeDisposable();

            var columns = Columns;
            if (columns is null)
            {
                return;
            }

            // Generate ColumnDefinitions

            var (MaxDeep, Items) = TemplatedDataGridColumnTopology.GetTopology(columns);

            var columnDefinitions = new List<ColumnDefinition>();

            for (var i = 0; i < Items.Count; i++)
            {
                var info = Items[i];
                var column = info.Column!;
                var isStarWidth = column.Width.IsStar;
                var isAutoWidth = column.Width.IsAuto;
                var isPixelWidth = column.Width.IsAbsolute;

                var columnDefinition = new ColumnDefinition();

                columnDefinition.OneWayBind(ColumnDefinition.MinWidthProperty, column, TemplatedDataGridColumn.MinWidthProperty, RootDisposables);
                columnDefinition.TwoWayBind(ColumnDefinition.MaxWidthProperty, column, TemplatedDataGridColumn.MaxWidthProperty, RootDisposables);

                if (isStarWidth)
                {
                    columnDefinition.OneWayBind(ColumnDefinition.WidthProperty, 
                        column.GetObservable(TemplatedDataGridColumn.ActualWidthProperty)
                                    .Select(x => new BindingValue<GridLength>(new GridLength(x, GridUnitType.Pixel))), 
                        RootDisposables);
                }

                if (isAutoWidth)
                {
                    columnDefinition.OneWayBind(ColumnDefinition.WidthProperty, column, TemplatedDataGridColumn.WidthProperty, RootDisposables);
                    columnDefinition.SetValue(DefinitionBase.SharedSizeGroupProperty, $"Column{i}");
                    
                    RootDisposables.Add(Disposable.Create(() =>
                    {
                        columnDefinition.SetValue(DefinitionBase.SharedSizeGroupProperty, default);
                    }));
                }

                if (isPixelWidth)
                {
                    columnDefinition.OneWayBind(ColumnDefinition.WidthProperty, column, TemplatedDataGridColumn.WidthProperty, RootDisposables);
                }

                columnDefinitions.Add(columnDefinition);

                // Generate DataGridCell's

                var cell = new TemplatedDataGridCell
                {
                    [Grid.ColumnProperty] = columnDefinitions.Count - 1
                };

                cell.TwoWayBind(TemplatedDataGridCell.SelectedItemProperty, this, TemplatedDataGridCellsPresenter.SelectedItemProperty, RootDisposables);
                cell.TwoWayBind(TemplatedDataGridCell.SelectedCellProperty, this, TemplatedDataGridCellsPresenter.SelectedCellProperty, RootDisposables);
                cell.OneWayBind(TemplatedDataGridCell.ContentProperty, this, TemplatedDataGridCellsPresenter.DataContextProperty, RootDisposables);
                cell.OneWayBind(TemplatedDataGridCell.CellTemplateProperty, column, TemplatedDataGridColumn.CellTemplateProperty, RootDisposables);
                
                _cells.Add(cell);
                _rootChildren.Add(cell);

                if (i < Items.Count)
                {
                    columnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Pixel)));
                }
            }

            columnDefinitions.Add(new ColumnDefinition(GridLength.Auto));

            _root.ColumnDefinitions.Clear();
            _root.ColumnDefinitions.AddRange(columnDefinitions);

            foreach (var child in _rootChildren)
            {
                _root.Children.Add(child);
            }
            
            RootDisposables.Add(Disposable.Create(() =>
            {
                foreach (var child in _rootChildren)
                {
                    _root.Children.Remove(child);
                }

                _root.RowDefinitions.Clear();
                _root.ColumnDefinitions.Clear();
            }));
        }

        internal void Detach()
        {
            RootDisposables?.Dispose();
            RootDisposables = null;

            if (_root is { })
            {
                _root.ColumnDefinitions.Clear();

                foreach (var child in _rootChildren)
                {
                    _root.Children.Remove(child);
                }
            }

            _cells.Clear();
        }
    }
}
