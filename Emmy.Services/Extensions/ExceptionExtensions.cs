using System;

namespace Emmy.Services.Extensions
{
    public class ExceptionExtensions
    {
        /// <summary>
        /// Ожидамая ошибка которая выбрасывается исключительно по вине пользователя.
        /// Выводится обратно пользователю и никак не логируется.
        /// </summary>
        public class GameUserExpectedException : Exception
        {
            public GameUserExpectedException()
            {
            }

            public GameUserExpectedException(string message)
                : base(message)
            {
            }

            public GameUserExpectedException(string message, Exception inner)
                : base(message, inner)
            {
            }
        }
    }
}