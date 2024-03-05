﻿using System;

namespace Cs4rsa.Common
{
    public abstract class CrawlUtils
    {
        /// <summary>
        /// GetByPaging time from Epoch
        /// 
        /// Mô tả:
        ///     Lấy ra thời gian đếm bằng milli giây kể từ ngày 01-01-1970.
        ///     Trong các server DTU, tham số t hay timestamp được sử
        ///     dụng để tránh việc lưu cache.
        ///     
        ///     -- Thanks to Son Tran --
        ///     -- Author: Script tự động đăng ký tín chỉ --
        /// </summary>
        public static string GetTimeFromEpoch()
        {
            var fromUnixEpoch = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            return fromUnixEpoch.ToString();
        }
    }
}
