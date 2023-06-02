/**
 * ETML
 * Author : Sébastien Duruz
 * Date : 25.05.2023
 * Description : JS scripts related to the PDF generation / print
 */

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