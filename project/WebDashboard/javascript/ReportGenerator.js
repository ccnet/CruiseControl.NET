document.getElementsByClassName = function(cl) {
  var retnode = [];
  var myclass = new RegExp('\\b'+cl+'\\b');
  var elem = this.getElementsByTagName('*');
  for (var i = 0; i < elem.length; i++) {
    var classes = elem[i].className;
    if (myclass.test(classes)) {
      retnode.push(elem[i]);
    }
  }
  return retnode;
};

function getSiblingNode(element) {
    do {
        element = element.nextSibling;
    } while (element && element.nodeType != 1);
    return element;
}

function toggleDetails() {
  var popup = getSiblingNode(this); 
  if (popup.style.display == 'block') { 
    popup.style.display = 'none'; 
  }
  else {
    var popups = document.getElementsByClassName('detailspopup');
    for (var i = 0, j = popups.length; i < j; i++) { 
      popups[i].style.display = 'none';
    }
    popup.style.display = 'block'; 
  }
  return false;
}

function collapseAllClasses() {
  var classRows = document.getElementsByClassName('classrow');
  for (var i = 0, j = classRows.length; i < j; i++) {
    classRows[i].style.display = 'none';
  }
  var expandedRows = document.getElementsByClassName('expanded');
  for (var i = 0, j = expandedRows.length; i < j; i++) {
    expandedRows[i].className = 'collapsed';
  }
  return false;
}

function expandAllClasses() {
  var classRows = document.getElementsByClassName('classrow');
  for (var i = 0, j = classRows.length; i < j; i++) {
    classRows[i].style.display = '';
  }
  var collapsedRows = document.getElementsByClassName('collapsed');
  for (var i = 0, j = collapsedRows.length; i < j; i++) {
    collapsedRows[i].className = 'expanded';
  }
  return false;
}

function toggleClassesInAssembly() {
  var assemblyRow = this.parentNode.parentNode;
  assemblyRow.className = assemblyRow.className == 'collapsed' ? 'expanded' : 'collapsed';
  var classRow = getSiblingNode(assemblyRow);
  while (classRow && classRow.className == 'classrow') {
    classRow.style.display = classRow.style.display == 'none' ? '' : 'none';
    classRow = getSiblingNode(classRow);
  }
  return false;
}

function init() {
  var toggleDetailsLinks = document.getElementsByClassName('toggleDetails');
  for (var i = 0, j = toggleDetailsLinks.length; i < j; i++) {
    toggleDetailsLinks[i].onclick = toggleDetails;
  }

  document.getElementById('collapseAllClasses').onclick = collapseAllClasses;
  document.getElementById('expandAllClasses').onclick = expandAllClasses;

  var toggleClassesInAssemblyLinks = document.getElementsByClassName('toggleClassesInAssembly');
  for (var i = 0, j = toggleClassesInAssemblyLinks.length; i < j; i++) {
    toggleClassesInAssemblyLinks[i].onclick = toggleClassesInAssembly;
  }
}

window.onload = init;