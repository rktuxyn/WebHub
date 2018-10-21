/**
* Copyright (c) 2018, SOW (https://www.facebook.com/safeonlineworld).  (https://github.com/RKTUXYN) All rights reserved.
* @author {SOW}
* @description {Hub Api Demo Client Side}
* @example { }
* Copyrights licensed under the New BSD License.
* See the accompanying LICENSE file for terms.
*/
( function ( $ ) {
	const HUB_NAME = "Manager";
	let __hub = Sow.Api.Hub.init( {
		pingInterval: 300000,
		waitForPageLoad: true,
		jsonp: false,
		withCredentials: true,
		transport: 'webSockets',
		//host: location.origin + /hub/,
		hubPath: "/hub/",
		crossDomain: true,
		logging: true
	} );
	Object.extend( __hub.client, {
		onConnected: function ( connectionId, hash, userName, data ) {
			if ( typeof ( data ) !== 'object' )
				data = JSON.parse( data );

			let out = "";
			for ( let i = 0, l = data.length; i < l; i++ ) {
				out += "\r\n" + String.format( '<li><a data-hash="{0}" href="#" {1}><span class="glyphicon glyphicon-user"> </span><span class="c_user">{2}</span></a></li>\r\n', data[i].hash, ( data[i].hash === hash ? 'disabled="disabled" class="disabled"' : "" ), data[i].user_name );
			}
			$( '[data-chat="user"]' ).html( out );

			Sow.hook( "___page" ).fire( "onConnected", [hash, userName] );
		},
		onNewUserConnected: function ( hash, connectionId, userName, time ) {
			$( '[data-chat="user"]' ).append( String.format( '<li><a data-hash="{0}" href="#"><span class="glyphicon glyphicon-user"></span><span class="c_user">{1}</span></a></li>\r\n', hash, userName ) );
		},
		onDisconnectUser: function ( hash, userName ) {
			$( '[data-chat="user"] [data-hash="' + hash + '"]' ).parent().remove();
		}
	} );

	Sow.hook( HUB_NAME ).add( "onReconnected", function ( hash, connectionId, userName, time ) {
		$( '[data-chat="user"]' ).append( String.format( '<li><a data-hash="{0}" href="#"><span class="glyphicon glyphicon-user"></span><span class="c_user">{1}</span></a></li>\r\n', hash, userName ) );
	} );

	$( document ).ready( function () {
		var _worker = {
			name: "", hash: "", chatHash: [],
			board: {
				getMsgBody: function ( m ) {
					return String.format(
						`<li class="maxtes">
											<a href="#" data-user="{0}">
												<span class="glyphicon glyphicon-user"></span> 
												<span class="text-muted"><span class="c_user">{1}</span> | {2}</span>
											</a>
											<div class="well">{3}</div></li>`,
						m.publish_hash, m.publisher_name, m.msg_date, m.message
					);
				},
				view: function ( msg ) {
					if ( msg === null )
						throw new Error( "Invalid public message defined!!!" );

					if ( typeof ( msg ) !== 'object' )
						msg = JSON.parse( msg );

					let msghtm = "";
					if ( $.isArray( msg ) ) {
						for ( let i = 0, l = msg.length; i < l; i++ ) {
							msghtm += this.getMsgBody( msg[i] );
						}
					} else if ( $.isPlainObject( msg ) ) {
						msghtm = this.getMsgBody( msg );
					}
					let $elm = $( '.board' );
					$elm.append( msghtm ).animate( { scrollTop: 400000 }, 1 );
					$elm = $elm.parent();
					$elm.animate( { scrollTop: 100000 }, "10" );
				}
			},
			writeMessage: function ( $el, msg ) {
				$el.append( msg );
				$el.scrollTop( $el[0].scrollHeight );
				return this;
			},
			getUserName: function ( hash ) {
				return $( ".connected-list" ).find( '[data-hash="' + hash + '"]' ).find( ".c_user" ).html().trim();
			},
			getFromMessage: function ( userName, msg ) {
				return '<div class="from_message"><b>' + userName + '</b><span class="msg">: ' + msg + '</span></div>';
			},
			getToMessage: function ( userName, msg ) {
				return '<div class="to_message"><b>' + userName + '</b><span class="msg">: ' + msg + '</span></div>';
			},
			createChatbox: function ( hash, name ) {
				console.log( hash );
				if ( hash === this.hash ) return;
				let $uel = $( '[data-chat="message-private"] [data-user="' + hash + '"]' );
				if ( $uel.length > 0 ) return $uel;
				$uel = $( '[data-chat="message-private"] table tbody' );
				let $tr = $( "<tr>" );
				$tr.html( "<td>" + String.format( $( '[data-type="message-template"]' ).html(), hash, name ) + "</td>" );
				$tr.find( '.close' ).click( function () {
					let index = _worker.chatHash.indexOf( hash );
					if ( index > -1 ) {
						_worker.chatHash.splice( index, 1 );
					}
					$tr.remove(); $tr = undefined;
				} );
				$tr.find( ".btnSendMessage" ).click( function () {
					let $textBox = $tr.find( ".txtPrivateMessage" );
					let msg = $textBox.val();
					if ( msg.length > 0 ) {
						_worker.writeMessage( $tr.find( ".messageArea" ), _worker.getFromMessage( _worker.name, msg ) );
						$textBox.val( '' );
						__hub.server.sendPrivateMessage( hash, msg );
					}
				} );
				$tr.find( ".txtPrivateMessage" ).keypress( function ( e ) {
					if ( e.which == 13 ) {
						$tr.find( ".btnSendMessage" ).click();
					}
				} ).on( "keyup", function ( e ) {
					e.preventDefault();
					//var msg = $( this ).val();
					setTimeout( function () {
						__hub.server.privateMessageKeyup( hash );
					}, 0 );
				} );
				this.chatHash.push( hash );
				$uel.prepend( $tr );
				return $tr;
			},
			loadPrivateMessage: function ( $el, toHash, msg ) {
				let resp = "", pd = "";
				let name = this.getUserName( toHash );
				for ( let i = 0, l = msg.length; i < l; i++ ) {
					let row = msg[i];
					if ( row.from_user_hash === this.hash ) {
						resp += this.getFromMessage( this.name, row.message );
						continue;
					}
					resp += this.getToMessage( name, row.message );
				}
				this.writeMessage( $el, resp );
			},
			regEvent: function () {
				$( '[data-chat="user"]' ).on( "click", function ( e ) {
					e.preventDefault();
					let $el = $( e.target );
					let tag = $el.prop( "tagName" );
					if ( tag === "LI" ) {
						$el = $el.find( 'a' );
					} else if ( tag === "SPAN" ) {
						$el = $el.parent();
					}
					let hash = $el.attr( 'data-hash' );
					if ( !hash ) return;
					let index = _worker.chatHash.indexOf( hash );
					if ( index >= 0 ) return;
					_worker.createChatbox( hash, $el.html() );
					__hub.server.loadPrivateMessage( hash );
				} );
				$( '[disabled]' ).removeAttr( "disabled" );
				$( '[data-chat="message-public"] form' ).submit( function ( e ) {
					e.preventDefault();
					let $input = $( '[type="text"]', this );
					let val = $input.val(); $input.val( "" );
					__hub.server.sendPublicMessage( val );
				} );
				$( '[data-chat="message-public"] [type="button"]' ).on( "click", function ( e ) {
					e.preventDefault();
					let $input = $( '[type="text"]', this );
					let val = $input.val(); $input.val( "" );
					__hub.server.sendPublicMessage( val );
				} );
			}
		};
		Sow.hook( "___page" ).add( "onConnected", function ( hash, userName ) {
			_worker.hash = hash; _worker.name = userName;
		} );
		Sow.hook( HUB_NAME ).add( "onPrivateMessage", function ( hash, userName, message ) {
			let $el = $( '[data-chat="message-private"]' ).find( '[data-user="' + hash + '"]' ).find( ".messageArea" );
			if ( $el.length <= 0 ) {
				_worker.createChatbox( hash, userName );
				__hub.server.loadPrivateMessage( hash );
				return;
			}
			_worker.writeMessage( $el, _worker.getToMessage( userName, message ) );
		} ).add( "onLoadPrivateMessage", function ( toHash, msg ) {
			if ( typeof ( msg ) !== 'object' ) {
				msg = JSON.parse( msg );
			}
			let $el = $( '[data-chat="message-private"]' ).find( '[data-user="' + toHash + '"]' ).find( ".messageArea" );
			if ( $el.length <= 0 )
				$el = _worker.createChatbox( toHash, _worker.getUserName( toHash ) );

			_worker.loadPrivateMessage( $el, toHash, msg );
			return;
		} ).add( "onPrivateMessageKeyup", function ( hash ) {

		} ).add( "onLoadPublicMessage", function ( message ) {
			_worker.board.view( message );
		} ).add( "onPublicMessage", function ( message ) {
			_worker.board.view( message );
		} );
		__hub.start().done( function () {
			_worker.regEvent();
			__hub.server.loadPublicMessage();
		} ).fail( function () {
			console.log( 'NOT_CONNECTED' );
		} );
	} );
}( jQuery ) );
