/********************
pullDown
********************/

;(function($){
	var namespace = 'pullDown';

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
					pulldownID : $(this).attr('aria-controles') // pulldownのidを格納
					,$pulldown : null // pulldownオブジェクト格納場所
					,expandedFlag : null // フラグ格納場所
				}, method));

				// dataコピー
				var $this = $(this)
				,options = $this.data(namespace);

				// options設定時に設定できなかった物をここで追加
				// pulldownオブジェクト
				options.$pulldown = $('#' + options.pulldownID);
				// aria-expanded取得
				options.expandedFlag = options.$pulldown.attr('aria-expanded');
				// boolear型にする
				options.expandedFlag = (options.expandedFlag == 'true') ? true : false;

				methods.eventSet.apply($this);

			});
		}

		/********************
		eventSet
		********************/
		,eventSet:function(){
			var $this = $(this)
			,options = $this.data(namespace)
			,$pulldownLi = options.$pulldown.find('li');

			$this.on('click.' + namespace, function(e){
				e.preventDefault();


				if(!options.expandedFlag){
					// 非表示状態のとき
					// フラグ反転
					options.expandedFlag = true;

					// 選択した時のイベントセット
					$pulldownLi.on('click.' + namespace , function(e){
						e.preventDefault();

						// ボタンの文字列を選択した文字列に上書き
						var selectText = $(e.target).filter('a').text();
						$this.text(selectText);
						// 隠す
						options.expandedFlag = false;
						options.$pulldown.attr('aria-expanded',options.expandedFlag);
					});
				}
				else{
					// 表示状態のとき
					// フラグ反転
					options.expandedFlag = false;
					// イベントを削除
					$pulldownLi.off('.' + namespace);
				}

				// フラグ上書き
				options.$pulldown.attr('aria-expanded',options.expandedFlag);


			})
		}

		/********************
		destroy
		********************/
		,destroy:function(){
			var $this = $(this)
			,options = $this.data(namespace)
			$pulldownLi = options.$pulldown.find('li');

			$this.off('.' + namespace);
			$pulldownLi.off('.' + namespace);
			// 隠す
			// options.$pulldown.attr('aria-expanded',true);

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