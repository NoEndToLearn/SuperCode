﻿using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public class BFieldComponentBase<TValue> : BComponentBase, IDisposable
    {
        [Parameter]
        public string Name { get; set; }
        [CascadingParameter]
        public BFormItem<TValue> FormItem { get; set; }

        protected void SetFieldValue(TValue value, bool validate)
        {
            if (FormItem == null)
            {
                return;
            }
            if (TypeHelper.Equal(FormItem.Value, value))
            {
                FormItem.MarkAsRequireRender();
                return;
            }
            Console.WriteLine($"设置 FormItem {Name} 值:" + value);
            FormItem.Value = value;
            if (!validate)
            {
                return;
            }

            FormItem.MarkAsRequireRender();
            FormItem.Validate();
            FormItem.ShowErrorMessage();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (FormItem != null)
            {
                FormItem.OnReset += FormItem_OnReset;
                Name = FormItem.Name;
            }
        }

        protected virtual void FormItem_OnReset(object value, bool requireRerender)
        {

        }

        public override void Dispose()
        {
            base.Dispose();
            if (FormItem != null)
            {
                FormItem.OnReset -= FormItem_OnReset;
            }
        }

    }
}
