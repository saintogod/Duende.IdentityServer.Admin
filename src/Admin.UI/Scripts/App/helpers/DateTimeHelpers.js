//Based on this article
//https://sebnilsson.com/blog/display-local-datetime-with-moment-js-in-asp-net/

$(function () {
  var formatter = new Intl.DateTimeFormat();
	$('.local-datetime').each(function () {
		var $this = $(this), utcDate = parseInt($this.attr('data-utc'), 10) || 0;

		if (!utcDate) {
			return;
		}

		$this.text(formatter.format(utcDate));
	});
	
	$('[data-toggle="tooltip"]').tooltip();

});