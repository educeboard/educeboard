/********************
listExpand
********************/

;(function($){
	var namespace = 'listExpand'
	,transitionend = 'transitionend.' + namespace
	 + ' webkitTransitionend.' + namespace
	 + ' msTransitionend.' + namespace;



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
					
					organizedList : {}

					,expandBtnObj : []

				}, method));

				// dataコピー
				var $this = $(this);
				
				methods.setEvent.apply($this);

			});
		}

		/********************
		eventSet
		********************/
		,setEvent:function(){
			var $this = $(this)
			,options = $this.data(namespace);

			$this.find('[aria-controles]').on('click.' + namespace, function(e){
				e.preventDefault();

				var $target = $(e.target)
				,$listObj = $target.closest('[data-' + namespace + '-level]')
				,expandName = $target.attr('aria-controles')
				,$expandObj = $listObj.find('#' + expandName)
				,targetExpanded = $target.attr('aria-selected')
				,$parentExpandObj = $target.closest('[aria-expanded]');

				var expandHeight = $expandObj.children().innerHeight();

				$expandObj.css('height', expandHeight + 'px');

				// クリックされたものと同じ階層のものを隠す
				$listObj.siblings('[data-' + namespace + '-level]').find('[aria-expanded]').attr('aria-expanded',false);
				$listObj.siblings('[data-' + namespace + '-level]').find('[aria-selected]').attr('aria-selected',false);
				$listObj.siblings('[data-' + namespace + '-level]').find('[aria-expanded]').css('height', '');


				if(targetExpanded == 'true'){
					// expandが閉じるとき
					$expandObj.attr('aria-expanded',false).css('height','');
					$target.attr('aria-selected',false);
					$listObj.find('[aria-expanded]').attr('aria-expanded',false);
					$listObj.find('[aria-selected]').attr('aria-selected',false);
				}
				else{
					// expandが開くとき
					$expandObj.attr('aria-expanded',true);
					$target.attr('aria-selected',true);
					// 親要素のheightを変更
					$expandObj.on(transitionend, function(){
						var parentHeight = $parentExpandObj.children().innerHeight();
						$parentExpandObj.css('height',parentHeight + 'px');

						// 全てのfalseのheightを初期化
						$this.find('[aria-expanded="false"]').css('height','');
					});
				}
			});

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