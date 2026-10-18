# Platform support

The included socket transport targets Unity platforms that expose `System.Net.Sockets` (Editor, Android, iOS, Windows, macOS and Linux). WebGL is intentionally excluded because browser networking is constrained; a WebGL deployment needs a browser-compatible provider such as WebSocket/WebTransport behind the same provider abstraction.
