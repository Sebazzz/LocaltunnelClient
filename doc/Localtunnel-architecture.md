# Localtunnel architecture

The architecture of localtunnel has (afaik) never been fully documented, so this is my attempt at it.

Localtunnel is used to circumvent firewalls by exposing a TCP port of the local computer through 
an man-in-the-middle, the so called localtunnel server. The process is roughly as follows:

1. On the local computer the localtunnel client is started, and makes a call to the localtunnel server with a requested hostname.
	a. The requested host name is optional, if not available or not provided the localtunnel server will assign a hostname.
	b. A request is made to `$server/$hostname` or `$server/?new`

2. The localtunnel server opens a TCP port for the localtunnel client, and assigns a hostname.
	a. The localtunnel server returns a object in the format of: `{ id: string; port: number; max_conn_count: number }`
	b. `max_conn_count` indicates then number of connections that can be opened to `$server:$port`
	c. The client has 1000ms to connect tunnels before the port is closed and hostname is freed.

3. The localtunnel client opens `max_conn_count` number of connections ("tunnels") to `$server:$port`
    a. Once the connection to the local tunnel server is open, a connection is made to the local server.
	   When the local connection is established, the local and remote connection are entangled so
	   that that any writes from the local connection are written remote and vice versa. In addition,
	   if the local or remote connection is closed the other connection is closed as well.
	b. When a tunnel closes, and the localtunnel client is not shutting down, a new tunnel is immediately opened. 

Localtunnel is mostly meant for HTTP connections, but I think the concept can be used for TCP connections as well. 

In any case, it should work fine for binary streams piped over HTTP. 