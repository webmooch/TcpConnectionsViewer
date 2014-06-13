#Tcp Connections Viewer


TCP Connections Viewer is a portable executable that displays detailed information about all TCP connections on the local system.

Existing mainstream connection viewer programs are generally closed-source and do not provide as much output-detail as I required.

TCP Connections Viewer is still in its infancy but more features and UI options are planned.

TCP Connections Viewer utilises v4.5 of the .NET framework and is written in C# with WPF MVVM.


##Purpose

Detecting and locating trojans, malware, etc

Securing your system by locating and closing listening ports

Analysing network connections made by any application

Terminating undesirable connections

Analyzing advanced remote IP information

Determining LAN MAC addresses


##Usage

When launched all TCP connection data is automatically populated and will refresh once per second.

Data can be sorted in ascending and descending order by clicking on any column title.

Columns can be displayed or hidden by right clicking on any column header and checking or unchecking the desired column name(s) from the context menu.

Certain process information is only available when the program is launched in elevated (administrative) mode, including the ability to terminate connections.


##Column Data

- Local Address
- Local Hostname
- Local Port
- Local Port Common Name
- Local Port Common Description
- Connection State
- Remote Address
- Remote MAC Address (LAN only)
- Remote MAC Address Manufacturer (LAN only)
- Remote Hostname
- Remote Port
- Remote Port Common Name
- Remote Port Common Description
- Remote Country
- Remote Isp
- Remote Dma Code
- Remote Timezone
- Remote Area Code
- Remote Asn
- Remote Continent Code
- Remote Longitude
- Remote Latitude
- Remote Country Code
- Remote Country Code3
- Process Id
- Process Caption
- Process Owner
- Process Creation Date
- Time Since Process Creation
- Process Executable Path
- Process Command Line


##Screenshot

![Main Screenshot](http://webmooch.net/images/TcpConnectionsViewerScreenshot.png)
