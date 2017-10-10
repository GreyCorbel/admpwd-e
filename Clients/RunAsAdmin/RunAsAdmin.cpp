// RunAsAdmin.cpp : main project file.

#include "stdafx.h"

using namespace System;
using namespace AdmPwd::PDSUtils;
using namespace AdmPwd::Types;

String^ GetBuiltInAdminName();

void Usage();

int main(array<System::String ^> ^args)
{
	String^ adminAccountName;
	String^ pathToExecutable;
	String^ domainName = nullptr;

	DWORD dwLogonFlags = LOGON_WITH_PROFILE;
	bool useLocalAccount = false;
	for each (String^ arg in args)
	{
		if (arg->StartsWith(L"/user:", StringComparison::CurrentCultureIgnoreCase))
		{
			adminAccountName = arg->Substring(6);
			useLocalAccount = false;
			continue;
		}
		if (arg->StartsWith(L"/path:", StringComparison::CurrentCultureIgnoreCase))
		{
			pathToExecutable = arg->Substring(6);
			continue;
		}
		if (arg->StartsWith(L"/noLocalProfile", StringComparison::CurrentCultureIgnoreCase))
		{
			dwLogonFlags = LOGON_NETCREDENTIALS_ONLY;
			continue;
		}
		if (arg->StartsWith(L"/?", StringComparison::CurrentCultureIgnoreCase))
		{
			Usage();
			return 0;
		}
	}
	try
	{
		//show usage when path not specified
		if (pathToExecutable == nullptr)
		{
			Console::WriteLine("ERROR: Path argument must be specified\n");
			Usage();
			return 0;
		}

		//if account not specified, use built-in admnin
		if (adminAccountName == nullptr)
		{
			adminAccountName = GetBuiltInAdminName();
			useLocalAccount = true;
			domainName = ".";
		}
		else
			if (adminAccountName->Contains("\\"))
			{
				array<String^>^ pairs = adminAccountName->Split('\\');
				domainName = pairs[0];
				adminAccountName = pairs[1];
			}
			else
				if (!adminAccountName->Contains("@"))
				{
					//non-built-in local account
					useLocalAccount = true;
					domainName = ".";
				}

		//get local computer name
		String^ computerName = System::Environment::GetEnvironmentVariable("COMPUTERNAME");
		PasswordInfo^ pi;
		if (useLocalAccount)
			//retrieve password for local computer
			pi = PdsWrapper::GetLocalAdminPassword(nullptr, computerName, false, false);
		else
			pi = PdsWrapper::GetManagedAccountPassword(nullptr, adminAccountName, false);

		//run desired process
		pin_ptr<const wchar_t> _adminAccount = PtrToStringChars(adminAccountName);
		pin_ptr<const wchar_t> _pwd = PtrToStringChars(pi->Password);
		pin_ptr<const wchar_t> _path = PtrToStringChars(pathToExecutable);
		pin_ptr<const wchar_t> _domain = nullptr;
		if (domainName != nullptr)
			_domain = PtrToStringChars(domainName);

		STARTUPINFO startInfo;
		PROCESS_INFORMATION processInfo;

		ZeroMemory(&startInfo, sizeof(startInfo));
		startInfo.cb = sizeof(startInfo);

		ZeroMemory(&processInfo, sizeof(processInfo));

		if (!CreateProcessWithLogonW(_adminAccount, _domain, _pwd, dwLogonFlags, nullptr, (LPWSTR)_path, CREATE_NEW_PROCESS_GROUP | ABOVE_NORMAL_PRIORITY_CLASS, nullptr, nullptr, &startInfo, &processInfo))
		{
			DWORD err = ::GetLastError();
			throw gcnew System::ComponentModel::Win32Exception(err);
		}
	}
	catch (PDSException^ ex)
	{
		Console::WriteLine("ERROR: " + ex->Message);
		return 1;
	}

	catch (System::ComponentModel::Win32Exception^ ex)
	{
		Console::WriteLine("ERROR: " + ex->NativeErrorCode.ToString() + ": " + ex->Message);
		return 1;
	}
	catch (Exception^ ex)
	{
		Console::WriteLine("ERROR: " + ex->Message);
		return 1;
	}
	return 0;
}

String^ GetBuiltInAdminName()
{
	PSID pSID = NULL;
	BOOL bResult = false;
	TCHAR *acctName = nullptr;
	TCHAR *RefDomain = nullptr;

	try
	{
		DWORD dwNameSize = 0, dwRefDomainSize = 0;
		SID_NAME_USE snu;

		//find out size of necessary buffers
		bResult = ConvertStringSidToSid(L"LA", &pSID);
		bResult = LookupAccountSid(NULL, pSID, NULL, &dwNameSize, NULL, &dwRefDomainSize, &snu);
		if (!bResult)
		{
			DWORD dwErr = GetLastError();
			if (dwErr != ERROR_INSUFFICIENT_BUFFER)
			{
				throw gcnew System::ComponentModel::Win32Exception(dwErr, L"Failed to find built-in admin account");
			}
		}

		//allocate buffers
		acctName = new(std::nothrow) TCHAR[dwNameSize];
		RefDomain = new(std::nothrow) TCHAR[dwRefDomainSize];
		if (acctName == NULL || RefDomain == NULL)
		{
			throw gcnew System::ComponentModel::Win32Exception(ERROR_NOT_ENOUGH_MEMORY, L"Failed to find built-in admin account");
		}

		//find out built-in admin account name
		bResult = LookupAccountSid(NULL, pSID, acctName, &dwNameSize, RefDomain, &dwRefDomainSize, &snu);
		if (!bResult)
		{
			throw gcnew System::ComponentModel::Win32Exception(GetLastError(), L"Failed to find built-in admin account");
		}
		return gcnew String(acctName);
	}
	finally
	{

		if (pSID)
			LocalFree(pSID);
		if (RefDomain != nullptr)
			delete[] RefDomain;
		if (acctName != nullptr)
			delete[] acctName;
	}
}

void Usage()
{
	Console::WriteLine("Usage:");
	Console::WriteLine("RunAsAdmin.exe /path:<path to executable to run> [/user:<name of admin account>] [/noLocalProfile]");
	Console::WriteLine("");
	Console::WriteLine("EXAMPLES:");
	Console::WriteLine("Runs command prompt as built-in local admin:");
	Console::WriteLine("  RunAsAdmin /path:%SystemRoot%\\system32\\cmd.exe");
	Console::WriteLine("");
	Console::WriteLine("Runs command prompt as domain account:");
	Console::WriteLine("  RunAsAdmin /path:%SystemRoot%\\system32\\cmd.exe /user:myaccount@mydomain.com");
	Console::WriteLine("");
	Console::WriteLine("Runs command prompt as custom local admin:");
	Console::WriteLine("  RunAsAdmin /path:%SystemRoot%\\system32\\cmd.exe /user:myCustomLocalAdmin");
	Console::WriteLine("");
	Console::WriteLine("Runs command prompt as domain user without creating local profile and without caching the password on local machine:");
	Console::WriteLine("  RunAsAdmin /path:%SystemRoot%\\system32\\cmd.exe /user:mydomain\\myaccount /noLocalProfile");
	Console::WriteLine("");


}
