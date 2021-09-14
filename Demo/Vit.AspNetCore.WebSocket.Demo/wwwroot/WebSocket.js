
; (() => {


	var webSocket;
	var host = "ws://" + location.host + "/websocket";
	var isOpen = false;

	function log(msg) {
		console.log(msg);
		txt.value += msg + '\r\n';
	}


	//创建websockt
	function CreateWebSocket() {
		webSocket = new WebSocket(host);
		webSocket.onopen = WebSokectOnOpen;
		webSocket.onmessage = WebSocketOnMessage;
		webSocket.onclose = WebSocketOnClose;
	}

	CreateWebSocket();
	setInterval(function () {
		if (isOpen) {
			var msg = '' + Math.random();
			log('websocket send:' + msg);
			webSocket.send(msg);
		}
	}, 500);

	function WebSokectOnOpen() {
		isOpen = true;
		log('websocket OnOpen');
	}


	function WebSocketOnMessage(event) {

		try {
			var msg = '' + event.data;
			log('websocket OnMessage:' + msg);
		} catch (e) {
		}
	}

	function WebSocketOnClose() {
		isOpen = false;
		log('websocket OnClose');
		CreateWebSocket();
	}



})();