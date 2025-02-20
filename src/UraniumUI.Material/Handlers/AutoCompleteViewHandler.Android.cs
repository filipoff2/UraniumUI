﻿#if ANDROID

using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using Google.Android.Material.TextField;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UraniumUI.Material.Controls;

namespace UraniumUI.Material.Handlers;
public partial class AutoCompleteViewHandler : ViewHandler<IAutoCompleteView, AppCompatAutoCompleteTextView>
{
    private AppCompatAutoCompleteTextView NativeControl => PlatformView as AppCompatAutoCompleteTextView;

    protected override AppCompatAutoCompleteTextView CreatePlatformView()
    {
        var autoComplete = new AppCompatAutoCompleteTextView(Context)
        {
            Text = VirtualView?.Text,
        };

        GradientDrawable gd = new GradientDrawable();
        gd.SetColor(global::Android.Graphics.Color.Transparent);
        autoComplete.SetBackground(gd);
        autoComplete.SetSingleLine(true);
        autoComplete.ImeOptions = ImeAction.Done;
        if (VirtualView != null)
        {
            autoComplete.SetTextColor(VirtualView.TextColor.ToAndroid());
        }

        return autoComplete;
    }
    protected override void ConnectHandler(AppCompatAutoCompleteTextView platformView)
    {
        PlatformView.TextChanged += PlatformView_TextChanged;
        PlatformView.EditorAction += PlatformView_EditorAction;
    }

    protected override void DisconnectHandler(AppCompatAutoCompleteTextView platformView)
    {
        PlatformView.TextChanged -= PlatformView_TextChanged;
        PlatformView.EditorAction -= PlatformView_EditorAction;
    }

    private void PlatformView_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
    {
        if (VirtualView.Text != PlatformView.Text)
        {
            VirtualView.Text = PlatformView.Text;
        }
    }

    private void PlatformView_EditorAction(object sender, TextView.EditorActionEventArgs e)
    {
        if (e.ActionId == Android.Views.InputMethods.ImeAction.Done)
        {
            VirtualView.Completed();
        }
    }

    private void SetItemsSource()
    {
        if (VirtualView.ItemsSource == null) return;

        ResetAdapter();
    }

    private void ResetAdapter()
    {
        var adapter = new BoxArrayAdapter(Context,
            Android.Resource.Layout.SimpleDropDownItem1Line,
            VirtualView.ItemsSource.ToList());

        NativeControl.Adapter = adapter;

        adapter.NotifyDataSetChanged();
    }

    public static void MapText(AutoCompleteViewHandler handler, AutoCompleteView view)
    {
        if (handler.NativeControl.Text != view.Text)
        {
            handler.NativeControl.Text = view.Text;
        }
    }

    public static void MapItemsSource(AutoCompleteViewHandler handler, AutoCompleteView view)
    {
        handler.SetItemsSource();
    }
}

internal class BoxArrayAdapter : ArrayAdapter
{
    private readonly IList<string> _objects;

    public BoxArrayAdapter(
        Context context,
        int textViewResourceId,
        List<string> objects) : base(context, textViewResourceId, objects)
    {
        _objects = objects;
    }
}

#endif