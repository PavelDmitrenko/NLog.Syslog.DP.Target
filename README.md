# NLog.Syslog.DP.Target

Syslog target for NLog logging platform.

## Supports
TCP non-encrypted transport.<br />
UDP & TLS is not supported.

## Settings
Host: IP-address of syslog server (_default 127.0.0.1_)  
Port: port of syslog server (_default 514_)

## Tested on
+ [Visual Syslog Server for Windows](http://maxbelkov.github.io/visualsyslog/)

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
