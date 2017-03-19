/********************
commentSyncScroll
********************/

;(function($){
	var namespace = 'commentSyncScroll'

	animationendEvent = 'webkitAnimationEnd.' + namespace + ' MSAnimationEnd.' + namespace + ' animationend.' + namespace;

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
					nowClass:'elNowComment'
					,$sendParts:null
					,$viewParts:null
					,sendPartsAttr:'sendArea'
					,viewPartsAttr:'viewArea'
					,viewAreaHeight:null
					
					,$timer:$('#Player [data-unitysyncdata-parts="timeView"]')
					,nowTime:0
					,$commentsDOM:null
					,commentArray:[]

					,educeboardBasicInfo:$('body').data('educeboardBasicInfo')

					,$textbox:$(this).find('[data-' + namespace + '-parts="textbox"]')
					,$submit:$(this).find('[data-' + namespace + '-parts="submit"]')

					,nowHeight:0

					,$checkbox:$(this).find('[data-' + namespace + '-parts="isSyncScroll"]')
					,isSyncScroll:true
					,syncScrollClass:'elSyncScroll'

					,commentClass:'elComment'
					,userClass:'elUser'
					,timestampClass:'elTimeStamp'
					,commentAddClass:'elAdded'

					,addedAnimationName:'addedCommentAnimation'

					,sendURL:'http://pb.fm.senshu-u.ac.jp/~tmochi/educeboard/insertComments.php'
					,sendData:{}

					,commentAdded:false
				}, method));

				// dataコピー
				var $this = $(this)
				,options = $this.data(namespace);

				options.isSyncScroll = options.$checkbox.prop('checked');

				console.log('isSyncScroll',options.isSyncScroll);

				options.$sendParts = $this.find('[data-' + namespace + '-parts="' + options.sendPartsAttr + '"]');
				options.$viewParts = $this.find('[data-' + namespace + '-parts="' + options.viewPartsAttr + '"]');

				options.$viewParts.toggleClass(options.syncScrollClass,options.isSyncScroll);
				options.viewAreaHeight = options.$viewParts.innerHeight();

				// console.log(options.)

				options.$commentsDOM = options.$viewParts.find('[data-' + namespace + '-timestamp]');
				var $comment;
				options.$commentsDOM.each(function(){
					$comment = $(this);
					options.commentArray.push({
						$comment:$comment
						,time:parseInt($comment.attr('data-' + namespace + '-timestamp'))
						,height:$comment.outerHeight()
					});
				})

				// console.log(options.commentArray[0].$comment);



				console.log('viewParts',options.$timer);
				console.log('commentsParts',options.$commentsDOM);

				methods.setEvent.apply($this);

			});
		}

		/********************
		setEvent
		********************/
		,setEvent:function(){
			var $this = $(this)
			,options = $this.data(namespace)
			,$textbox = options.$textbox
			,$submit = options.$submit
			,val
			,$checkbox = options.$checkbox;

			// console.log($checkbox);

			console.log($submit);

			$submit.on('click.' + namespace,function(e){

				e.preventDefault();

				val = $textbox.val();

				// console.log(val);

				console.log(val);

				if(val === undefined || val === '' || val === null){
					return false;
				}



				methods.sendComment.apply([$this,val]);

			});

			$checkbox.on('click.' + namespace,function(e){
				var $target = $(this);

				options.isSyncScroll = $target.prop('checked');
				options.$viewParts.toggleClass(options.syncScrollClass,options.isSyncScroll);

				// console.log('checkbox');

				if(options.isSyncScroll){

					options.$viewParts.children().css(prefixedStyle({
						transform:'translateY(' + (-1 * options.$viewParts.scrollTop()) + 'px)'
					}))
					.delay(1)
					.queue(function(){
						options.$viewParts.scrollTop(0)
						options.$viewParts.children().css(prefixedStyle({
							transform:'translateY(' + options.nowHeight + 'px)'
							,transition:''
						}))
						.dequeue();

					})
					

					
				}
				else{
					options.$viewParts.children().css(prefixedStyle({
						transform:''
						,transition:'none'
					}));
					options.$viewParts.scrollTop(-1 * options.nowHeight);
					options.$viewParts.animate({scrollTop:-1 * options.nowHeight},'fast');
				}
			});
		}

		/********************
		commentSync
		********************/
		,commentSync:function(){
			var $this = $(this)
			,options = $this.data(namespace)
			// ,$commentDOM
			,timestamp
			// ,prevTimestamp = options.prevTimestamp
			,nowTime
			,nowTimestampResult = false
			,resultCommentArray = []
			,everCommentArray = []
			,moveHeight = 0
			,nowTimePosition = 0;

			options.nowTime = options.$timer.attr('data-unitySyncData-time');

			nowTime = parseInt(options.nowTime);

			var commentArray = options.commentArray
			,commentArrayLength = commentArray.length
			,i
			,j;

			for(i = 0; i < commentArrayLength; i++){
				everCommentArray.push(commentArray[i]);

				// moveHeight -= commentArray[i].$comment.outerHeight();

				if(nowTime === commentArray[i].time){

					// options.commentAdded = false;

					// console.log(nowTime,commentArray[i].time);

					resultCommentArray.push(commentArray[i]);

					for(j = i + 1; j < commentArrayLength; j++){
						
						if(nowTime == commentArray[j].time){
							everCommentArray.push(commentArray[j]);
							resultCommentArray.push(commentArray[j]);

							// moveHeight -= commentArray[i].$comment.outerHeight();
						}
						else{
							break;
						}
					}

					nowTimestampResult = true;

					break;
				}
			}

			// console.log(resultCommentArray);
			// console.log(everCommentArray);

			// console.log('nowTimestampResult',nowTimestampResult);


			var resultNum = resultCommentArray.length
			,evenNum = everCommentArray.length
			,moveBorder = 0;

			if(nowTimestampResult){
				options.$commentsDOM.toggleClass(options.nowClass,false);
				for(i = 0; i < resultNum; i++){
					resultCommentArray[i].$comment.toggleClass(options.nowClass,true);
				}

				for(i = evenNum - 1; i >= 0; i--){
					moveBorder -= everCommentArray[i].height;

					// console.log(-1 * options.viewAreaHeight,moveHeight,i,everCommentArray[i]);
					if((-1 * options.viewAreaHeight) > moveBorder){

						for(j = 0; j <= i; j++){
							moveHeight -= everCommentArray[j].height;
						}

						if(options.isSyncScroll){
							options.$viewParts.children().css(prefixedStyle({
								transform:'translateY(' + moveHeight + 'px)'
							}));
						}

						options.nowHeight = moveHeight;

						break;
					}
				}
			}


		}

		/********************
		sendComment
		********************/
		,sendComment:function(){
			var $this = $(this[0])
			,val = this[1]
			,options = $this.data(namespace)
			,$textbox = options.$textbox
			,nowTime = parseInt(options.$timer.attr('data-unitySyncData-time'))
			,educeboardBasicInfo = options.educeboardBasicInfo
			,commentArray = options.commentArray
			,commentArrayLength = commentArray.length
			,i
			,j
			,resultIndex
			,timeArray = [0,0,0]
			,isLast = false;

			//ajaxの処理入れる

			$.ajax({
				type:'POST'
				,url:options.sendURL
				,data:{
					uid:educeboardBasicInfo.uid
					,session_id:educeboardBasicInfo.sid
					,tid:educeboardBasicInfo.tid
					,time:nowTime
					,comments:this[1]
				}
			}).done(function(xml){

				console.log(xml);

				//commentの中身を削除
				$textbox.val('');

				//現在のコメントの添字を検索
				for(i = 0; i < commentArrayLength; i++){

					//コメントのtimeがnowTime以上 かつ commentArrayが最後ではなかったら現在の添字を登録
					if(nowTime <= commentArray[i].time && i + 1 !== commentArray.length){

						isLast = false;
						resultIndex = i;
						break;

					}
					//そうじゃなかったら毎回最後を入れておく（ifが通らなかったらコメントが最後になっているとみなす）
					else{

						isLast = true;
						resultIndex = commentArray.length - 1;

					}
				}


				//時間をhtmlに表示するために文字列変換を行う
				timeArray[2] = nowTime;
				timeArray[1] = parseInt(timeArray[2] / 60);
				timeArray[0] = parseInt(timeArray[1] / 60);

				timeArray[1] = timeArray[1] - 60 * timeArray[0];
				timeArray[2] = timeArray[2] - 60 * timeArray[1];

				// console.log(nowTime);

				var str = ''
				,timeArrayLength = timeArray.length;
				for(i = 0; i < timeArrayLength; i++){
					if(timeArray[i] < 10){
						timeArray[i] = '0' + timeArray[i];
					}

					str += timeArray[i];

					if(i != timeArrayLength - 1){
						str += ':';
					}
				}


				//DOMの生成
				var $commentWrapper = $('<li>').attr('data-' + namespace + '-timestamp',nowTime).toggleClass(options.commentAddClass,true)
				,$dl = $('<dl />')
				,$comment = $('<dt />').toggleClass(options.commentClass,true).text(val)
				,$user = $('<dd />').toggleClass(options.userClass,true).text(options.educeboardBasicInfo.uname)
				,$timestamp = $('<dd />').toggleClass(options.timestampClass,true).text(str);

				$dl.append($comment).append($user).append($timestamp);

				$commentWrapper.append($dl);

				//コメントが最後じゃなかったら、resultIndexより前にDOMを追加
				if(!isLast){
					commentArray[resultIndex].$comment.before($commentWrapper);

					commentArray.splice(resultIndex,0,{
						$comment:$commentWrapper
						,time:nowTime
						,height:$commentWrapper.outerHeight()
					});
				}
				//コメントが最後じゃなかったら、最後だったら一番最後に追加
				else{
					commentArray[resultIndex].$comment.after($commentWrapper);

					commentArray.push({
						$comment:$commentWrapper
						,time:nowTime
						,height:$commentWrapper.outerHeight()
					});
				}
				

				// $commentWrapper.toggleClass(options.commentAddClass,false);

				$commentWrapper.on(animationendEvent,function(e){

					$commentWrapper.off(animationendEvent);

					if(e.originalEvent.animationName === options.addedAnimationName){

						$commentWrapper.toggleClass(options.commentAddClass,false);
					}
				});

				options.$commentsDOM = options.$viewParts.find('[data-' + namespace + '-timestamp]');
				options.commentArray = commentArray;

				methods.commentSync.apply($this);

			}).fail(function(){
				alert('コメントの送信に失敗しました');
			});

			// console.log(commentArray);
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

		/********************
		destroy
		********************/
		,destroy:function(){
			

		}
	};

	/********************
	prefixedStyle
	ベンダープレフィックス付きのスタイルを返す関数
	********************/
	function prefixedStyle(style){

		var prefix = ['ms','webkit','moz']
		,result = {}
		,i,j
		,prefixLength = prefix.length;

		for(i in style){

			for(j = 0; j < prefixLength; j++){
				result['-' + prefix[j] + '-' + i] = style[i];
			}

			result[i] = style[i];

		}

		return result;
	}

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