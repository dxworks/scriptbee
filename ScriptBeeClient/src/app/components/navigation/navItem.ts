import { Permission } from '../../types/permissions';

export interface NavItem {
  link: string;
  name: string;
  icon: string;
  children?: NavItem[];
  permission: Permission;
}
