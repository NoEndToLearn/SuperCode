﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class BButton : BComponentBase
    {
        internal HtmlPropertyBuilder cssClassBuilder;
        protected async Task OnButtonClickedAsync(MouseEventArgs e)
        {
            if (IsDisabled)
            {
                return;
            }
            var oldImg = showingImage;
            if (!string.IsNullOrWhiteSpace(showingImage) && !string.IsNullOrWhiteSpace(ClickImage))
            {
                showingImage = ClickImage;
                StateHasChanged();
                await Task.Delay(100);
            }
            if (OnClick.HasDelegate)
            {
                await OnClick.InvokeAsync(e);
            }
            if (!string.IsNullOrWhiteSpace(oldImg))
            {
                showingImage = oldImg;
                StateHasChanged();
            }
        }

        private string showingImage;

        /// <summary>
        /// 文本
        /// </summary>
        [Parameter]
        public string Text { get; set; }

        /// <summary>
        /// 按钮图片
        /// </summary>
        [Parameter]
        public string Image { get; set; }

        /// <summary>
        /// 鼠标放上去时的按钮图片
        /// </summary>
        [Parameter]
        public string HoverImage { get; set; }

        /// <summary>
        /// 鼠标点击时的按钮图片
        /// </summary>
        [Parameter]
        public string ClickImage { get; set; }

        /// <summary>
        /// 是否将自定义的 CSS 类加入到已有 CSS 类，如果为 false，则替换掉默认 CSS 类，默认为 true
        /// </summary>
        [Parameter]
        public bool AppendCustomCls { get; set; } = true;
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        [Parameter]
        public ButtonType Type { get; set; } = ButtonType.Default;

        [Parameter]
        public bool IsPlain { get; set; }

        [Parameter]
        public bool IsRound { get; set; }

        [Parameter]
        public bool IsDisabled { get; set; }

        [Parameter]
        public bool IsCircle { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public Size Size { get; set; } = GlobalElementSettings.Size;

        [Parameter]
        public string Icon { get; set; }

        [Parameter]
        public bool IsLoading { get; set; }

        public void ShowLoading()
        {
            IsLoading = true;
        }

        public void HideLoading()
        {
            IsLoading = false;
        }

        protected override bool ShouldRender()
        {
            return true;
        }

        private void MouseOver()
        {
            if (string.IsNullOrWhiteSpace(HoverImage))
            {
                return;
            }
            showingImage = HoverImage;
        }
        private void MouseOut()
        {
            if (string.IsNullOrWhiteSpace(HoverImage))
            {
                return;
            }

            showingImage = Image;
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (string.IsNullOrWhiteSpace(showingImage) && !string.IsNullOrWhiteSpace(Image))
            {
                showingImage = Image;
            }
            IsDisabled = IsLoading;
            cssClassBuilder = HtmlPropertyBuilder.CreateCssClassBuilder();
            if (string.IsNullOrWhiteSpace(Cls) || AppendCustomCls)
            {
                cssClassBuilder.Add($"el-button", $"el-button--{Type.ToString().ToLower()}", Cls)
                .Add($"el-button--{Size.ToString().ToLower()}")
                .AddIf(IsPlain, "is-plain")
                .AddIf(IsRound, "is-round")
                .AddIf(IsDisabled, "is-disabled")
                .AddIf(IsLoading, "is-loading")
                .AddIf(IsCircle, "is-circle");
                return;
            }
            cssClassBuilder.AddIf(!string.IsNullOrWhiteSpace(Cls), Cls);
            if (string.IsNullOrWhiteSpace(Cls))
            {
                cssClassBuilder.Add($"el-button", $"el-button--{Type.ToString().ToLower()}")
                    .Add($"el-button--{Size.ToString().ToLower()}")
                    .AddIf(IsPlain, "is-plain")
                    .AddIf(IsRound, "is-round")
                    .AddIf(IsDisabled, "is-disabled")
                    .AddIf(IsLoading, "is-loading")
                    .AddIf(IsCircle, "is-circle");
            }
        }
    }
}
