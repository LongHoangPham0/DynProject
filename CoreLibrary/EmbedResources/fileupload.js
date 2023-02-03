function removeUploadFile(element) {
    //element
    var parent = element.parentNode.parentNode;
    var input = document.createElement('input');
    input.type = 'file';
    input.name = element.getAttribute('name');
    parent.appendChild(input);

    parent.removeChild(element.parentNode);
    return false;
}