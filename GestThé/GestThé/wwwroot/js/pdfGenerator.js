/**
 * ETML
 * Author : Sébastien Duruz
 * Date : 25.05.2023
 * Description : JS scripts related to the PDF generation / print
 */

/**
 * Download the pdf file
 * @param filename
 * @param byteBase64
 */
function downloadPdf(filename, byteBase64){
    let link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + byteBase64;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

/**
 * Show the PDF document in iframe preview box
 * @param iframeId
 * @param byteBase64
 * @constructor
 */
function ViewPdf(iframeId, byteBase64) {
    document.getElementById(iframeId).innerHTML = "";
    let iframe = document.createElement('iframe');
    iframe.setAttribute("src", "data:application/pdf;base64," + byteBase64);
    iframe.style.width = '760px';
    iframe.style.height = '800px';
    document.getElementById(iframeId).appendChild(iframe);
}

/**
 * Convert the content of the PDF to a readable file
 * @param b64Data
 * @returns {Blob}
 */
function base64Blob(b64Data) {
    sliceSize = 512;
    let byteCharacters = atob(b64Data);
    let byteArrays = [];
    for (var offset = 0; offset < byteCharacters.length; offset += sliceSize) {
        var slice = byteCharacters.slice(offset, offset + sliceSize);
        var byteNumbers = new Array(slice.length);
        for (var i = 0; i < slice.length; ++i) {
            byteNumbers[i] = slice.charCodeAt(i);
        }
        var byteArray = new Uint8Array(byteNumbers);
        byteArrays.push(byteArray);
    }
    var blob = new Blob(byteArrays, { type: 'application/pdf' });
    return blob;
}