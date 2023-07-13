# TestGameState
Test task for GameState

Примерное время работы ~25 часов

Приницип работы:
1. Граф задается в SkillTree с помощью прямых ссылок
2. GameLoader парсит ссылки в матрицу смежности и отправляет данные в GamePresenter->GameModel
3. GameModel отвечает за состояние способностей и поинты
4. SkillTreeView обрабатывает ввод и отправляет события, на которые подписан GamePresenter
5. GamePresenter обновляет свою модель, на которую подписан SkillTreePresenter
6. SkillTreePresenter обновляет свою модель, на которую подписан SkillTreeView