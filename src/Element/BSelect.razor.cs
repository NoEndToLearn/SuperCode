﻿




using Element.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Element
{
    public partial class BSelect<TValue> : BFieldComponentBase<TValue>, IDisposable
    {
        protected internal bool isTree;
        private HtmlPropertyBuilder warpperClsBuilder;
        internal ElementReference elementSelect;
        private Type valueType;
        private Type nullable;
        internal bool isClearable = true;
        internal bool EnableClearButton { get; set; }

        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public Action<string> LabelChanged { get; set; }
        internal ObservableCollection<SelectResultModel<TValue>> Options { get; set; } = new ObservableCollection<SelectResultModel<TValue>>();

        [Parameter]
        public TValue InitialValue { get; set; }
        [Parameter]
        public string Placeholder { get; set; } = "请选择";
        [Parameter]
        public EventCallback<TValue> ValueChanged { get; set; }

        /// <summary>
        /// 当绑定为枚举时，指定哪些枚举名需要忽略
        /// </summary>
        [Parameter]
        public string[] IgnoreEnumNames { get; set; } = new string[0];

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            warpperClsBuilder = HtmlPropertyBuilder.CreateCssClassBuilder()
                .Add("el-select", Cls);
            if (valueType == null)
            {
                InitilizeEnumValues(FormItem != null);
            }
            if (FormItem == null)
            {
                Label = Label ?? Options.FirstOrDefault(x => TypeHelper.Equal(x.Key, Value))?.Text;
                return;
            }

            if (FormItem.OriginValueHasRendered)
            {
                return;
            }
            FormItem.OriginValueHasRendered = true;
            if (FormItem.Form.Values.Any())
            {
                Value = FormItem.OriginValue;
            }

            if (dict != null && Value != null)
            {
                Label = dict[Value] ?? Label;
            }
            SetFieldValue(Value, false);
        }

        private void InitilizeEnumValues(bool firstItemAsValue)
        {
            valueType = typeof(TValue);
            nullable = Nullable.GetUnderlyingType(valueType);
            isClearable = nullable != null;
            valueType = nullable ?? valueType;
            var valueSet = false;
            if (valueType.IsEnum)
            {
                var names = Enum.GetNames(valueType);
                var values = Enum.GetValues(valueType);
                dict = new Dictionary<TValue, string>();
                for (int i = 0; i < names.Length; i++)
                {
                    var name = names[i];
                    if (IgnoreEnumNames.Contains(name, StringComparer.CurrentCultureIgnoreCase))
                    {
                        continue;
                    }
                    var value = values.GetValue(i);
                    var field = valueType.GetField(name);
                    var text = field.GetCustomAttributes(typeof(DescriptionAttribute), true)
                        .Cast<DescriptionAttribute>()
                        .FirstOrDefault()?.Description ?? name;
                    if (!valueSet && firstItemAsValue)
                    {
                        valueSet = true;
                        if (nullable == null)
                        {
                            if (!TypeHelper.Equal(Value, (TValue)value))
                            {
                                Value = (TValue)value;
                                InitialValue = Value;
                                SetFieldValue(Value, false);
                            }
                            Label = text;
                        }
                    }
                    dict.Add((TValue)value, text);
                }
                ChildContent = builder =>
                {
                    int seq = 0;
                    foreach (var itemValue in dict.Keys)
                    {
                        builder.OpenComponent<BSelectOption<TValue>>(seq++);
                        builder.AddAttribute(seq++, "Text", dict[itemValue]);
                        builder.AddAttribute(seq++, "Value", itemValue);
                        builder.CloseComponent();
                    }
                };
            }
        }

        public void UpdateValue(string text)
        {
            var option = Options.FirstOrDefault(x => x.Text == text);
            if (option == null)
            {
                Value = default;
            }
            else
            {
                Value = option.Key;
            }
            if (ValueChanged.HasDelegate)
            {
                _ = ValueChanged.InvokeAsync(Value);
            }
            if (TypeHelper.Equal(Value, default))
            {
                SelectedOption = null;
            }
            SetFieldValue(Value, true);
        }
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Inject]
        internal PopupService PopupService { get; set; }

        /// <summary>
        /// 当前选中值
        /// </summary>
        [Parameter]
        public TValue Value { get; set; }

        [Parameter]
        public EventCallback<BChangeEventArgs<SelectResultModel<TValue>>> OnChange { get; set; }

        [Parameter]
        public EventCallback<BChangeEventArgs<SelectResultModel<TValue>>> OnChanging { get; set; }

        internal SelectResultModel<TValue> SelectedOption
        {
            get
            {
                return selectedOption;
            }
            set
            {
                if (value == null)
                {
                    Value = default;
                    Label = string.Empty;
                }
                else
                {
                    Value = value.Key;
                    Label = value.Text;
                }
                selectedOption = value;
                LabelChanged?.Invoke(Label);
                if (ValueChanged.HasDelegate)
                {
                    _ = ValueChanged.InvokeAsync(Value);
                }

            }
        }

        private SelectResultModel<TValue> selectedOption;
        internal DropDownOption dropDownOption;
        private Dictionary<TValue, string> dict;

        internal async Task OnInternalSelectAsync(SelectResultModel<TValue> item)
        {
            var args = new BChangeEventArgs<SelectResultModel<TValue>>();
            args.NewValue = item;
            args.OldValue = SelectedOption;
            if (OnChanging.HasDelegate)
            {
                await OnChanging.InvokeAsync(args);
                if (args.DisallowChange)
                {
                    return;
                }
            }

            await dropDownOption.Instance.CloseDropDownAsync(dropDownOption);
            SelectedOption = item;
            SetFieldValue(item.Key, true);
            Value = item.Key;
            if (dict != null)
            {
                dict.TryGetValue(Value, out var label);
                Label = label;
            }
            if (OnChange.HasDelegate)
            {
                await OnChange.InvokeAsync(args);
            }
            Console.WriteLine(Label);
            EnableClearButton = false;
            StateHasChanged();
        }

        internal void OnSelectClick(MouseEventArgs e)
        {
            if (EnableClearButton)
            {
                EnableClearButton = false;
                return;
            }
            if (PopupService.SelectDropDownOptions.Any(x => x.Target.Id == elementSelect.Id))
            {
                return;
            }

            dropDownOption = new DropDownOption()
            {
                IsTree = isTree,
                Select = this,
                Target = elementSelect,
                OptionContent = ChildContent,
                Refresh = () =>
                {
                    StateHasChanged();
                },
                IsShow = true
            };
            PopupService.SelectDropDownOptions.Add(dropDownOption);
        }

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            var enumValue = (TValue)value;
            if (nullable != null && value == null)
            {
                SelectedOption = null;
            }
            else
            {
                Label = dict?[enumValue];
                Value = enumValue;
            }
            if (ValueChanged.HasDelegate)
            {
                _ = ValueChanged.InvokeAsync(Value);
            }
            else
            {
                StateHasChanged();
            }
        }
        protected override bool ShouldRender()
        {
            return true;
        }
    }
}
