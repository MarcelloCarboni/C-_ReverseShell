# C# Reverse Shell

This is a simple reverse shell written in C# to remotely execute powershell commands on the victim's machine.

If the connection fails for the client, it will try to reconnect for up to 5 times, waiting 30 seconds after each attempt.

It also features a server, written using C#.
If you don't want to use this server, yuo can simply use netcat like this `nc -lvp 1234`
