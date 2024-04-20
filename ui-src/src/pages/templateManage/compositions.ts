import { getEmailTemplatesCount, getEmailTemplatesData } from 'src/api/emailTemplate'
import { useQTable } from 'src/compositions/qTableUtils'
import { IQtableRequestParams, TTableFilterObject } from 'src/compositions/types'

// 查看缩略图
import 'viewerjs/dist/viewer.css'
import { api as viewerApi } from 'v-viewer'

// 生成邮件模板数据
export function useEmailTemplateTable () {
  async function getRowsNumberCount (filterObj: TTableFilterObject) {
    const { data } = await getEmailTemplatesCount(filterObj.filter)
    return data
  }
  async function onRequest (filterObj: TTableFilterObject, pagination: IQtableRequestParams) {
    const { data } = await getEmailTemplatesData(filterObj.filter as string, pagination)
    return data
  }

  const { pagination, ...others } = useQTable({
    getRowsNumberCount,
    onRequest
  })

  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  function getTemplateImage (row: Record<string, any>) {
    if (!row) return '/public/icons/undraw_mailbox_re_dvds.svg'
    let baseUrl = process.env.BASE_URL as string
    baseUrl = baseUrl.replace('/api/v1', '')
    const url = baseUrl + '/' + row.thumbnail
    // console.log(url)
    return url
  }

  // 查看缩略图
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  function onPreviewThumbnail (row: Record<string, any>) {
    const imageUrl = getTemplateImage(row)

    viewerApi({
      options: {
        title: false,
        transition: true,
        navbar: false,
        zIndex: 9999
      },
      images: [imageUrl]
    })
  }

  return {
    pagination,
    getTemplateImage,
    onPreviewThumbnail,
    ...others
  }
}

/**
 * 获取 WYSIWYG (what you see is what you get) 编辑器配置
 */
export function useWysiwygEditor () {
  const editorDefinitions = {}

  const editorToolbar = [
    ['left', 'center', 'right', 'justify'],
    ['bold', 'italic', 'strike', 'underline', 'link'],
    ['undo', 'redo'],
    ['viewsource']
  ]

  return { editorDefinitions, editorToolbar }
}
