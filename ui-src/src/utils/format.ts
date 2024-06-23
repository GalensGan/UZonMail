import dayjs from 'dayjs'

/**
 * 格式化字符串型日期
 * @param dateStr
 * @param format
 * @returns
 */
export function formatDateStr (dateStr: string, format = 'YYYY-MM-DD HH:mm:ss') {
  if (!dateStr) return ''
  if (dateStr.startsWith('0001')) return ''
  console.log('dateStr', dateStr)
  return dayjs(dateStr).format(format)
}
