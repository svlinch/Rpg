Unity 2021.3.16f1

Смотреть удобнее с:
На сцене SceneContext -> Loader
В ассетах -> Scripts -> Initialization -> Loader
	     		Logic -> GamePlayController


Все игровые шаблоны и настройки в Json-файлах в папке Resources.

UnitModel - хранит бафы, модели способностей и два варианта шаблона юнита: ИЗМЕНЯЕМЫЙ и неизменяймый. 

Все параметры(health, armor,...) хранятся в этих шаблонах, в словарях Dictionary<string, ParameterTemplate<float>>.

Результат применения способности(GamePlayController -> HandleSkillClicked) - класс BattleResult, хранящий два словаря Dictionary<string, float>. 

1 словарь для изменений в ИЗМЕНЯЕМОМ шаблоне атакующего юнита(для вампиризма или повышения брони, например)

2 словарь для изменений в шаблоне юнита на другой стороне(урон, урон по броне и т.д.)