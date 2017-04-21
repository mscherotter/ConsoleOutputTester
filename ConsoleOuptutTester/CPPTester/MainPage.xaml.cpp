//
// MainPage.xaml.cpp
// Implementation of the MainPage class.
//

#include "pch.h"
#include "MainPage.xaml.h"
#include <ppl.h>
using namespace concurrency;
using namespace CPPTester;

using namespace Platform;
using namespace Windows::ApplicationModel::AppService;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::System;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

MainPage::MainPage()
{
	InitializeComponent();

	Unloaded += ref new Windows::UI::Xaml::RoutedEventHandler(this, &CPPTester::MainPage::OnUnloaded);

	Connect();
}

void MainPage::Connect() 
{
	auto uri = ref new Uri("consoleoutput:");

	create_task(Launcher::LaunchUriAsync(uri)).then([this](bool launched) 
	{
		if (launched)
		{
			auto connection = ref new AppServiceConnection();
			connection->AppServiceName = "consoleoutput";
			connection->PackageFamilyName = "49752MichaelS.Scherotter.ConsoleOutput_9eg5g21zq32qm";

			create_task(connection->OpenAsync()).then([connection, this](AppServiceConnectionStatus status)
			{
				auto message = ref new ValueSet();
				
				message->Insert("Message", "Hello Console Tester");

				connection->SendMessageAsync(message);

				this->_connection = connection;
			});
		}
	});
}

void CPPTester::MainPage::OnUnloaded(Platform::Object ^sender, Windows::UI::Xaml::RoutedEventArgs ^e)
{
}


void CPPTester::MainPage::SendMessage(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	if (this->_connection != nullptr)
	{
		auto message = ref new ValueSet();

		message->Insert("Message", this->Message->Text);

		_connection->SendMessageAsync(message);

		this->Message->Text = "";
	}
}
