# NLog.Syslog.DP.Target

### Syslog target for NLog logging platform

## Dependencies
+ .NET Standart (>= 2.0)
+ NLog (>= 4.5.9)

## Features
+ Supports TCP non-encrypted transport.<br />
_Note: UDP & TLS is NOT supported._
+ Correctly handles cyrillic encoding on windows and \*nix platforms

## Release history
**v.1.2**
* TCP Socket reusability
* Additional settings for multiplatform encoding support (win, linux, osx)

**v.1.1**
* Minor fixes

**v.1.0**
* Initial release

## Usage
+ Install from [nuget](https://www.nuget.org/packages/NLog.Syslog.DP.Target/)
+ Add reference to extensions section in nlog config file (_see example below_)
+ Specify new syslog target
+ Start logging using native NLog functionality

## Settings
**Host**<br>
IP-address of syslog server (_default 127.0.0.1_)  

**Port**<br>
Port of syslog server (_default 514_)

**EncodingGlobal**<br>
Encoding used for logs if no platform-specific settings exists (_default utf-8_)

**EncodingOnLinuxOS**<br>
Encoding used for logs on Linux platfrom (_default utf-8_)

**EncodingOnWindowsOS**<br>
Encoding used for logs on Windows platfrom (_default utf-8_)

**EncodingOnOSXOS**<br>
Encoding used for logs on OSX platfrom (_default utf-8_)

## Tests passed
+ Windows-hosted application → [Visual Syslog Server for Windows](http://maxbelkov.github.io/visualsyslog/)
+ CentOS-hosted application → native \*nix SysLog

## Example NLog config file:
```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
	<extensions>
		<add assembly="NLog.Syslog.DP.Target"/>
	</extensions>
	
	<targets>
		<target name="syslog" xsi:type="Syslog" host="127.0.0.1" port="514" />
		<target name="logconsole" xsi:type="Console" />
	</targets>

	<rules>
		<logger name="*" minlevel="Debug" writeTo="logconsole,syslog" />
	</rules>
</nlog>
```
