﻿
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public abstract class BFormItemObject : BComponentBase
    {
        /// <summary>
        /// 是否应用样式，如果不应用，则该组件本身不生成任何 HTML
        /// </summary>
        [Parameter]
        public bool ApplyStyle { get; set; } = true;
        /// <summary>
        /// 初始值是否已渲染
        /// </summary>
        public bool OriginValueHasRendered { get; set; } = false;
        /// <summary>
        /// 初始值是否已设置
        /// </summary>
        internal bool OriginValueHasSet { get; set; } = false;
        [Parameter]
        public string Label { get; set; }

        /// <summary>
        /// 设置字段 Label 为图片地址
        /// </summary>
        [Parameter]
        public string Image { get; set; }

        /// <summary>
        /// 标签宽度
        /// </summary>
        [Parameter]
        public float LabelWidth { get; set; } = 100;

        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public bool IsRequired { get; set; }

        [Parameter]
        public string RequiredMessage { get; set; }
        internal bool IsShowing { get; set; } = true;

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// 占位符
        /// </summary>
        [Parameter]
        public string Placeholder { get; set; }
        [CascadingParameter]
        public BForm Form { get; set; }

        public IList<IValidationRule> Rules { get; set; } = new List<IValidationRule>();
        public ValidationResult ValidationResult { get; protected set; }

        protected override void OnInitialized()
        {
            Form.Items.Add(this);
            var validation = Form.Validations.FirstOrDefault(x => x.Name == Name);
            if (validation != null)
            {
                Rules = validation.Rules;
            }
            if (IsRequired && !Rules.OfType<RequiredRule>().Any())
            {
                var requiredRule = new RequiredRule();
                requiredRule.ErrorMessage = RequiredMessage ?? $"请确认{Label}";
                Rules.Add(requiredRule);
            }
        }

        internal void ShowErrorMessage()
        {
            if (ValidationResult == null || ValidationResult.IsValid)
            {
                return;
            }
            IsShowing = true;
            _ = Task.Delay(100).ContinueWith((task) =>
            {
                IsShowing = false;
                MarkAsRequireRender();
                InvokeAsync(StateHasChanged);
            });
        }
        public virtual void Validate()
        {

        }


        public virtual void Reset()
        {

        }
    }
}
