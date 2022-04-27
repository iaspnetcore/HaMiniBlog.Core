
## source project

remote: https://github.com/iaspnetcore/HaMiniBlog.Core.git

local:  F:\developer_msly\Miniblog.Core

domain:home-assisant.msly.cn

host:vweb2 /var/www/msly/Miniblog.Core/src

## software version

tinymce: 4.8.0 (2018-06-27)


## Useful links

https://blogifier.net/docs/getting-started/

https://github.com/anjoy8/Blog.Core

kestrel-home-assisant.msly.cn

3.服务启动

kestrel-home-assisant.msly.cn.service

~~~
sudo vi /etc/systemd/system/kestrel-home-assisant.msly.cn.service

sudo systemctl enable kestrel-home-assisant.msly.cn.service
sudo systemctl start kestrel-home-assisant.msly.cn.service
sudo systemctl restart kestrel-home-assisant.msly.cn.service
sudo systemctl stop kestrel-home-assisant.msly.cn.service
sudo systemctl status kestrel-home-assisant.msly.cn.service

sudo journalctl -fu kestrel-home-assisant.msly.cn.service


sudo systemctl daemon-reload 

sudo systemctl restart kestrel-home-assisant.msly.cn.service





--------------------tool.msly.cn debug-----------------------------
sudo systemctl stop kestrel-home-assisant.msly.cn.service

cd /
cd  /var/www/msly/Miniblog.Core/src

dotnet run --urls http://localhost:4699

------release

cd /
cd /var/www/msly/Miniblog.Core/src

dotnet publish -c release


cd /var/www/msly/Miniblog.Core/src/bin/release/net6.0/publish/

dotnet Miniblog.Core.dll --urls http://localhost:4699


4.批处理

sudo systemctl stop kestrel-home-assisant.msly.cn.service

cd /var/www/msly/Miniblog.Core/src

dotnet publish -c release

sudo systemctl daemon-reload 

sudo systemctl restart kestrel-home-assisant.msly.cn.service

~~~