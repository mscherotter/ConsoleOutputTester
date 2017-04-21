//
// MainPage.xaml.h
// Declaration of the MainPage class.
//

#pragma once

#include "MainPage.g.h"

using namespace Windows::ApplicationModel::AppService;

namespace CPPTester
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public ref class MainPage sealed
	{
	public:
		MainPage();
	private:
		void Connect();

		AppServiceConnection^ _connection;
		void OnUnloaded(Platform::Object ^sender, Windows::UI::Xaml::RoutedEventArgs ^e);
		void SendMessage(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
	};
}
