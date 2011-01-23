function checkForParams(buttonEl, checkUrl){
  $('#parameterEditor').replaceWith('<div id="parameterEditor">Loading parameters, please wait...</div>');
  $('#parameterCheck').dialog('open');
  jQuery.get(checkUrl, function(data){
      if(data=='NONE'){
        $('#parameterCheck').dialog('close');
        var button = $(buttonEl);
        button.after('<input type="hidden" name="ForceBuild" value="Force" />');
        buttonEl.parentNode.parentNode.submit();
      }else{
        $('#parameterEditor').replaceWith('<div id="parameterEditor">' + data + '</div>');
      }
    });
}
