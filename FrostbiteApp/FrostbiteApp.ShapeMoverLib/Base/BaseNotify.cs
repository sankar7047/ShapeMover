using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace FrostbiteApp.ShapeMoverLib.Base
{
	public class BaseNotify : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Sets the property changed with the field reference
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="currentValue"></param>
		/// <param name="newValue"></param>
		/// <param name="propertyName"></param>
		/// <param name="forceRefresh"></param>
		/// <returns></returns>
		public bool SetPropertyChanged<T>(ref T currentValue, T newValue, [CallerMemberName] string propertyName = "", bool forceRefresh = false)
		{
			return PropertyChanged.SetProperty(this, ref currentValue, newValue, propertyName, forceRefresh);
		}

		/// <summary>
		/// Sets the property chanded with the property name
		/// </summary>
		/// <param name="propertyName"></param>
		public void SetPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}

namespace System.ComponentModel
{
	public static class BaseNotify
	{
		/// <summary>
		/// Extension method of SetProperty to accept reference field
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="handler"></param>
		/// <param name="sender"></param>
		/// <param name="currentValue"></param>
		/// <param name="newValue"></param>
		/// <param name="propertyName"></param>
		/// <param name="forceRefresh"></param>
		/// <returns></returns>
		public static bool SetProperty<T>(this PropertyChangedEventHandler handler, object sender, ref T currentValue, T newValue, [CallerMemberName] string propertyName = "", bool forceRefresh = false)
		{
			if (!forceRefresh && EqualityComparer<T>.Default.Equals(currentValue, newValue))
				return false;

			currentValue = newValue;

			if (handler == null)
				return true;

			handler.Invoke(sender, new PropertyChangedEventArgs(propertyName));
			return true;
		}
	}
}
