# miniblog.core-ipv6


## source project

remote: https://github.com/iaspnetcore/HaMiniBlog.Core/tree/miniblog.core-ipv6

local:  F:\developer_msly\Miniblog.Core

domain:www.ipv6address.info

host:vweb2 /var/www/msly/Miniblog.Core/src

## software version

tinymce: 4.8.0 (2018-06-27)

publish first

~~~
cd /var/www/Miniblog.Core-wwwipv6addressinfo/Miniblog.Core/src/bin/Release/net6.0/publish/wwwroot

chmod ./posts 777

~~~


## Useful links

https://blogifier.net/docs/getting-started/

https://github.com/anjoy8/Blog.Core

cd /var/www/Miniblog.Core-wwwipv6addressinfo/Miniblog.Core/src

3.服务启动

kestrel-wwwipv6addressinfo.service

~~~
sudo vi /etc/systemd/system/kestrel-wwwipv6addressinfo.service

sudo systemctl enable kestrel-wwwipv6addressinfo.service
sudo systemctl start kestrel-wwwipv6addressinfo.service
sudo systemctl restart kestrel-wwwipv6addressinfo.service
sudo systemctl stop kestrel-wwwipv6addressinfo.service
sudo systemctl status kestrel-wwwipv6addressinfo.service

sudo journalctl -fu kestrel-wwwipv6addressinfo.service


sudo systemctl daemon-reload 

sudo systemctl restart kestrel-wwwipv6addressinfo.service





-------------------- debug-----------------------------

sudo systemctl stop kestrel-wwwipv6addressinfo.service


cd  /var/www/Miniblog.Core-wwwipv6addressinfo/Miniblog.Core/src

dotnet run --urls http://localhost:7600

------  release  ----


cd /var/www/Miniblog.Core-wwwipv6addressinfo/Miniblog.Core/src

dotnet publish -c Release


sudo cp -R /var/www/Miniblog.Core-wwwipv6addressinfo/Miniblog.Core/src/bin/release/net6.0/publish/wwwroot/Posts/* /var/www/Miniblog.Core-wwwipv6addressinfo/Miniblog.Core/src/wwwroot/Posts

cd  /var/www/Miniblog.Core-wwwipv6addressinfo/Miniblog.Core/src/bin/Release/net6.0/publish/

dotnet Miniblog.Core.dll --urls http://localhost:7600


4.批处理

sudo systemctl stop kestrel-wwwipv6addressinfo.service

sudo cp -R /var/www/Miniblog.Core-wwwipv6addressinfo/Miniblog.Core/src/bin/Release/net6.0/publish//wwwroot/Posts/* /var/www/Miniblog.Core-wwwipv6addressinfo/Miniblog.Core/src/wwwroot/Posts

cd /var/www/Miniblog.Core-wwwipv6addressinfo/Miniblog.Core/src

dotnet publish -c Release

sudo systemctl daemon-reload 

sudo systemctl restart kestrel-wwwipv6addressinfo.service

~~~


~~~
kestrel-wwwipv6addressinfo.service

[Unit]
Description=www.ipv6address.info App running on Ubuntu

[Service]
WorkingDirectory= /var/www/Miniblog.Core-wwwipv6addressinfo/Miniblog.Core/src/bin/Release/net6.0/publish/
ExecStart=/usr/bin/dotnet /var/www/Miniblog.Core-wwwipv6addressinfo/IPV6/Miniblog.Core-master/src/bin/release/net5.0/publish/Miniblog.Core.dll --urls http://127.0.0.1:7600
Restart=always
# Restart service after 10 seconds if the dotnet service crashes:
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=dotnet-example
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target



~~~