async function CopyUrlToClipboard(path){
    var newURL = window.location.protocol + "//" + window.location.host + path;
    await navigator.clipboard.writeText(newURL);
}