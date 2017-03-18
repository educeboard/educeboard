/********************
seekBar
********************/

;(function($){
	var namespace = 'seekBar';

	var $body = $('body');

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
					$position : $(this).find('[data-' + namespace + 'parts="position"]')
					,$timeLength : $(this).find('[data-' + namespace + 'parts="timeLength"]')
					,startX : null
					,startY : null
					,moveX : null
					,moveY : null
					,endX : null
					,endY : null
				}, method));

				// dataコピー
				var $this = $(this)
				,options = $this.data(namespace);

				methods.eventSet.apply($this);

			});
		}

		/********************
		eventSet
		********************/
		,eventSet:function(){
			var $this = $(this)
			,options = $this.data(namespace)
			,$position = options.$position;

			console.log($position);

			$position.on('mousedown.' + namespace,function(e){
				console.log('mousedown');
				methods.moveSeek.apply($this);
			});
			
		}

		/********************
		moveSeek
		********************/
		,moveSeek:function(){
			var $this = $(this)
			,options = $this.data(namespace)
			,$position = options.$position;

			$this.on('mousemove.' + namespace,function(e){
				console.log(e.clientX);
			});
		}

		/********************
		unMoveSeek
		********************/
		,unMoveSeek:function(){
			var $this = $(this)
			,options = $this.data(namespace)
			,$position = options.$position;

			$this.off('mousemove.' + namespace);
		}

		/********************
		destroy
		********************/
		,destroy:function(){
			var $this = $(this)
			,options = $this.data(namespace);
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