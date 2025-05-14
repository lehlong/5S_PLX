import { BaseFilter } from '../base.model'

export class HeSoMatHangFilter extends BaseFilter {
  id: string = ''
  goodsCode: string = ''
  isActive?: boolean | string | null
  SortColumn: string = ''
  IsDescending: boolean = true
}
