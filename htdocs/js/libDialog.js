/********************
libDialog
********************/

;(function($){
	var namespace = 'libDialog';


	/********************
	methods
	********************/

	var methods = {

		/********************
		initialize
		********************/
		initialize:function(method){
			return this.each(function(){

				// オプションをセット
				$(this).data(namespace, $.extend(true, {

					$closeOverlay : $('<div />')

					,closeOverlayClass : 'elCloseOverlay'
					,overlayAnotherAttr : null

					,$show : null
					,$close : null

					,dialogID : $(this).attr('id').substring(0)

					,setEvent : true

					,setEventFlag : false

					// callback
					,closeAfter : null

				}, method));

				// dataコピー
				var $this = $(this)
				,options = $this.data(namespace);

				console.log('initialize libDialog');
				
				methods.prepare.apply($this);

			});
		}

		/********************
		prepare
		********************/
		,prepare:function(){
			var $this = $(this)
			,options = $this.data(namespace)
			,split = []

			//IDが取得できなかったら強制終了
			if(options.dialogID === undefined){
				return;
			}

			if(options.overlayAnotherAttr !== null && typeof options.overlayAnotherAttr === 'string'){

				split = options.overlayAnotherAttr.replace(/\"/g,'').split('=');

				options.$closeOverlay.attr(split[0],split[1]);


			}

			options.$closeOverlay
			.toggleClass(options.closeOverlayClass,true)
			.attr('data-' + namespace + '-parts','close')
			.attr('aria-controles',options.dialogID)

			$this.prepend(options.$closeOverlay);

			options.$show = $('[data-' + namespace + '-parts="show"][aria-controles="' + options.dialogID + '"]');
			options.$close = $('[data-' + namespace + '-parts="close"][aria-controles="' + options.dialogID + '"]');

			if(options.setEventFlag){
				methods.setEvent.apply($this);
			}

			
		}

		/********************
		setEvent
		********************/
		,setEvent:function(){
			var $this = $(this)
			,options = $this.data(namespace);

			options.setEventFlag = true;


			options.$show.on('click.' + namespace, function(e){
				e.preventDefault();

				methods.showDialog.apply($this);
			});

			options.$close.on('click.' + namespace,function(e){
				e.preventDefault();

				methods.closeDialog.apply($this);
			});


		}

		/********************
		showDialog
		********************/
		,showDialog:function(){
			var $this = $(this)
			,options = $this.data(namespace);

			$this.attr('aria-hidden',false);

			if(!options.setEventFlag){
				methods.setEvent.apply($this);
			}
		}

		/********************
		closeDialog
		********************/
		,closeDialog:function(){
			var $this = $(this)
			,options = $this.data(namespace);

			$this.attr('aria-hidden',true);

			if(!options.setEventFlag){
				methods.setEvent.apply($this);
			}

			methods.applyCallback.apply([$this,'closeAfter']);
		}

		/********************
		destroy
		********************/
		,destroy:function(){
			var $this = $(this)
			,options = $this.data(namespace);

			console.log(options)

			options.$show.off('.' + namespace);
			options.$close.off('.' + namespace);
			options.$closeOverlay.remove();
			$this.attr('aria-hidden',true);

			$this.removeData(namespace);
		}

		/********************
		applyCallback
		********************/
		,applyCallback:function(){
			var $this = $(this)[0]
			,options = $this.data(namespace)
			,callback = $(this)[1];


			if(typeof options[callback] === 'function'){;
				options[callback]();
			}
		}
	};


	/********************
	全プラグイン共通
	********************/
	$.fn[namespace] = function(method){
		if(methods[method]){
			return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
		}
		else if(typeof method === 'object' || !method){
			return methods.initialize.apply(this, arguments);
		}
	}

})(jQuery);