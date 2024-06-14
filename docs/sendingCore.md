# 邮件发送开发文档

## 优先级

- 主题

  Excel数据/subject  > 界面/主题

- 正文

  Excel数据/body > Excel数据/templateId > Excel数据/templateName > 界面/正文 > 界面/模板 

- 发件人

  Excel数据/outbox > 界面/发件人

- 收件人

  Excel数据/inbox > 界面/收件人

- 抄送人

  Excel数据/cc > 界面/抄送人

- 附件

  附件目前无法通过数据指定

## 数据有效性

在发件时，数据按如下步骤检查有效性：

1. 必须有主题，因为主题相当于本次发件的名称
2. 必须具备能从既有数据中解析出发件人、收件人、正文