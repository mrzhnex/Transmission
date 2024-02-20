Логика: .net standart
UI:
- Windows: .net core + wpf
- Linux: .net core + wpf + mono / .net core + gui library

Типы операционных систем:
Персональные компьютеры: Windows (7, 8, 10), Linux, MacOS;

Тип программы: Многопользовательский аудио-мессенджер (аудио коммуникатор).

Вид коммуникации: Передача пакетов цифровых данных – преобразованных аудио-сигналов – открытым или закрытым (метод шифрования) способом.

Особенности коммуникации: 
    1) подключение большого количества клиентов к серверу по двухуровневому типу;
    2) все клиенты слышат друг друга и могут говорить одновременно;

Клиентская программа. Минимальные настройки:
    1) включение/выключение динамиков;
    2) включение/выключение микрофона;
    3) регулировка уровня воспроизводства звука в динамиках;
    4) регулировка уровня чувствительности микрофона;
    5) запись трансляции аудио-потока (формат аудио-файла MP3);

Серверная программа. Интерфейс сервера:
    1) список клиентов:
    • IP-адрес;
    • время подключения к серверу;
    • микрофон включен/выключен;
    • динамики включены/выключены;
    2) сортировка списка клиентов по:
    • времени подключения;
    • IP-адресу;
    3) заглушение исходящего звука от клиента;
    4) заглушение входящего звука к клиенту;
    5) запись трансляции аудио-потока (формат аудио-файла MP3);
    6) включение/выключение шифрования передачи аудио;
