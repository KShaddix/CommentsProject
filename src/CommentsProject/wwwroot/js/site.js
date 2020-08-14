$(function () {
  $('*[id^=answer_').bind('click', function () {
    var numberId = $(this).attr('id').substr(7);
    var articleId = $('#articleId').attr('data-article-id');

    var commentElement = $('#comment_' + numberId);
    if (commentElement.children('#answer-form_' + numberId).length > 0)
      commentElement.children('#answer-form_' + numberId)[0].remove();
    else
      commentElement.append(`
        <div id="answer-form_${numberId}" class="border-top mt-3 pt-2">
            <form action="/Article/${articleId}" method="post">
                <div>
                  <label class="article__comment-label">ID пользователя:</label>
                  <input type="number" name="userId" />
                </div>
                <div>
                    <label class="article__comment-label">Комментарий:</label>
                    <textarea name="text" rows="5" cols="30"></textarea>
                </div>
                <input type="hidden" name="articleId" value="${articleId}" />
                <input type="hidden" name="parentId" value="${numberId}" />
                <div>
                  <input type="submit" value="Отправить" />
                </div>
            </form>
        </div>
      `);
    }
  )
});