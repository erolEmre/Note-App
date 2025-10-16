//window.markAsDone = function (noteId) {
//    console.log("AJAX çağrısı başladı:", noteId);
//    var token = $('input[name="__RequestVerificationToken"]').val();

//    $.ajax({
//        url: '/Note/MarkAsDone',
//        type: 'POST',
//        data: { noteId: noteId, __RequestVerificationToken: token },
//        success: function (response) {
//            if (response.success) {
//                $('#note-status-' + noteId).text('Done');
//                console.log("Status güncellendi");
//            } else {
//                alert(response.message);
//            }
//        },
//        error: function () {
//            alert('Bir hata oluştu.');
//        }
//    });
//}
// Notu "Done" olarak işaretle
window.markAsDone = function (noteId) {
    sendStatusChange(noteId, 'MarkAsDone');
};

// Notu "Planned" olarak işaretle
window.markAsPlanned = function (noteId) {
    sendStatusChange(noteId, 'MarkAsPlanned');
};

// Notu "ToDo" olarak işaretle
window.markAsToDo = function (noteId) {
    sendStatusChange(noteId, 'MarkAsToDo');
};

// Ortak AJAX isteği (tekrar eden kodu ortadan kaldırır)
function sendStatusChange(noteId, actionName) {
    var token = $('input[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: `/Note/${actionName}`,
        type: 'POST',
        data: {
            noteId: noteId,
            __RequestVerificationToken: token
        },
        success: function (response) {
            if (response.success) {
                // Sayfada id'si note-status-<noteId> olan elementi güncelle
                $('#note-status-' + noteId).text(response.newStatus);
            } else {
                alert(response.message || 'Bir hata oluştu.');
            }
        },
        error: function (xhr) {
            console.error(xhr.responseText);
            alert('Sunucu hatası oluştu.');
        }
    });
}
